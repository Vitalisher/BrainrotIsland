using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Unity.AI.Navigation;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    [Header("Цели")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float targetSearchInterval = 0.5f;

    [Header("Преследование")]
    [SerializeField] private float chaseSpeed = 5f;
    [SerializeField] private float detectionRange = 15f;
    [SerializeField] private float loseTargetRange = 25f;

    [Header("Атака")]
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float attackDelay = 0.5f;

    [Header("Анимация")]
    [SerializeField] private Animator animator;
    [SerializeField] private string runAnimParam = "isRunning";
    [SerializeField] private string jumpAnimParam = "Jump";
    [SerializeField] private string attackAnimParam = "Attack";

    private enum EnemyState { Idle, Chase, Jumping, Attack }

    private EnemyState state;
    private NavMeshAgent agent;
    private Transform target;

    private float nextSearchTime;
    private bool canAttack = true;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (!animator) animator = GetComponentInChildren<Animator>();
        agent.speed = chaseSpeed;
        state = EnemyState.Idle;
    }

    void Update()
    {
        if (Time.time >= nextSearchTime)
        {
            FindNearestPlayer();
            nextSearchTime = Time.time + targetSearchInterval;
        }

        if (!target)
        {
            state = EnemyState.Idle;
            return;
        }

        float dist = Vector3.Distance(transform.position, target.position);

        switch (state)
        {
            case EnemyState.Idle:
            case EnemyState.Chase:
                HandleChase(dist);
                break;
            case EnemyState.Jumping:
                // Прыжок обрабатывается NavMeshAgent автоматически
                break;
            case EnemyState.Attack:
                HandleAttack(dist);
                break;
        }

        UpdateAnimations();
    }

    void FindNearestPlayer()
    {
        Collider[] players = Physics.OverlapSphere(transform.position, loseTargetRange, playerLayer);

        float min = Mathf.Infinity;
        Transform closest = null;

        foreach (var col in players)
        {
            if (!col.CompareTag("Player")) continue;

            float d = Vector3.Distance(transform.position, col.transform.position);
            if (d < min)
            {
                min = d;
                closest = col.transform;
            }
        }

        target = closest;
    }

    void HandleChase(float dist)
    {
        if (dist <= attackRange)
        {
            state = EnemyState.Attack;
            return;
        }

        if (agent.enabled && agent.isOnNavMesh)
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);
        }

        state = EnemyState.Chase;
    }

    void HandleAttack(float dist)
    {
        if (dist > attackRange * 1.2f)
        {
            state = EnemyState.Chase;
            return;
        }

        if (canAttack)
            StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        canAttack = false;
        animator?.SetTrigger(attackAnimParam);

        yield return new WaitForSeconds(attackDelay);

        if (target && Vector3.Distance(transform.position, target.position) <= attackRange)
            target.GetComponent<ITakeDamage>()?.TakeDamage(attackDamage);

        yield return new WaitForSeconds(attackCooldown - attackDelay);
        canAttack = true;
    }

    void UpdateAnimations()
    {
        if (!animator) return;

        animator.SetBool(runAnimParam, state == EnemyState.Chase);
    }

    // Этот метод будет вызван автоматически, когда агент начнет использовать NavMeshLink для прыжка
    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<NavMeshLink>() != null)
        {
            animator?.SetTrigger(jumpAnimParam);
        }
    }
}
