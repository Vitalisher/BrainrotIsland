using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshBotController : MonoBehaviour
{
    public Health health;
    public Animator animator;

    [Header("Movement")]
    public float walkSpeed = 3.5f;
    public float sprintSpeed = 6f;
    public float roamRadius = 40f;
    public float pointReachDistance = 1.5f;

    [Header("Jump")]
    public string jumpTrigger = "Jump";

    private NavMeshAgent agent;
    private Vector3 targetPoint;
    private bool hasTarget;
    private bool isJumping;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = animator ?? GetComponentInChildren<Animator>();

        agent.speed = walkSpeed;
        PickRandomPoint();
    }

    void Update()
    {
        if (health != null && health.isDead)
            return;

        if (agent == null || !agent.enabled || !agent.isOnNavMesh)
        {
            UpdateAnimatorStopped();
            return;
        }

        HandleMovement();
        UpdateAnimator();
    }

    void HandleMovement()
    {
        if (!hasTarget || agent.remainingDistance <= pointReachDistance)
        {
            PickRandomPoint();
        }
    }

    void PickRandomPoint()
    {
        if (!agent.enabled || !agent.isOnNavMesh)
            return;

        Vector3 randomDirection = Random.insideUnitSphere * roamRadius;
        randomDirection += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, roamRadius, NavMesh.AllAreas))
        {
            targetPoint = hit.position;
            agent.SetDestination(targetPoint);
            agent.speed = Random.value > 0.5f ? sprintSpeed : walkSpeed;
            hasTarget = true;
        }
    }

    void UpdateAnimator()
    {
        float speed = agent.velocity.magnitude;
        animator.SetBool("isRunning", speed > 0.1f && !isJumping);
    }

    void UpdateAnimatorStopped()
    {
        animator.SetBool("isRunning", false);
    }

    void OnTriggerEnter(Collider other)
    {
        // Проверяем, если бот начал использовать NavMeshLink
        if (other.GetComponent<NavMeshLink>() != null)
        {
            isJumping = true;
            animator.SetTrigger(jumpTrigger);
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Проверяем, если бот закончил использовать NavMeshLink
        if (other.GetComponent<NavMeshLink>() != null)
        {
            isJumping = false;
        }
    }
}
