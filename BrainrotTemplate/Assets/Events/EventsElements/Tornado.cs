using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Tornado : MonoBehaviour
{
    [Header("Движение")]
    public float moveSpeed = 3f;
    public float minMoveTime = 2f;
    public float maxMoveTime = 5f;
    public float rotationSpeed = 50f;

    [Header("Земля")]
    public LayerMask groundLayer;
    public float groundRayDistance = 50f;
    public float groundOffset = 0.1f;
    public float groundCheckDistance = 5f; // Дистанция проверки земли впереди

    [Header("Подъём")]
    public float liftRadius = 8f;
    public float pullForce = 10f;
    public float liftForce = 25f;
    public float maxLiftHeight = 20f;

    [Header("Отключение")]
    public float disableRadius = 3f;
    public float disableTime = 5f;

    [Header("Урон")]
    public int damagePerSecond = 10;
    public float damageRadius = 3f;

    public LayerMask playerLayer;

    private Vector3 currentDirection;
    private float nextDirectionChangeTime;

    private HashSet<GameObject> disabledPlayers = new HashSet<GameObject>();
    private Dictionary<GameObject, float> damageTimers = new Dictionary<GameObject, float>();

    void Start()
    {
        ChooseNewDirection();
        StickToGround();
    }

    void Update()
    {
        MoveTornado();
        StickToGround();

        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        LiftPlayers();
        DisablePlayersInRadius();
        DamagePlayers();
        ClearDamageTimers();
    }

    void MoveTornado()
    {
        if (Time.time >= nextDirectionChangeTime)
            ChooseNewDirection();

        // Проверяем, есть ли земля впереди перед движением
        Vector3 nextPosition = transform.position + currentDirection * moveSpeed * Time.deltaTime;
        
        if (IsGroundAhead(nextPosition))
        {
            Vector3 move = currentDirection * moveSpeed * Time.deltaTime;
            move.y = 0f;
            transform.position += move;
        }
        else
        {
            // Если земли нет впереди, сразу выбираем новое направление
            ChooseNewDirection();
        }
    }

    bool IsGroundAhead(Vector3 position)
    {
        // Проверяем наличие земли в следующей позиции
        Ray ray = new Ray(position + Vector3.up * 5f, Vector3.down);
        return Physics.Raycast(ray, groundRayDistance, groundLayer);
    }

    void StickToGround()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 5f, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, groundRayDistance, groundLayer))
        {
            Vector3 pos = transform.position;
            pos.y = hit.point.y + groundOffset;
            transform.position = pos;
        }
    }

    void ChooseNewDirection()
    {
        int maxAttempts = 20; // Максимум попыток найти направление с землёй
        
        for (int i = 0; i < maxAttempts; i++)
        {
            float angle = Random.Range(0f, 360f);
            Vector3 direction = new Vector3(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                0,
                Mathf.Sin(angle * Mathf.Deg2Rad)
            ).normalized;

            // Проверяем несколько точек по направлению
            Vector3 checkPosition = transform.position + direction * groundCheckDistance;
            
            if (IsGroundAhead(checkPosition))
            {
                currentDirection = direction;
                nextDirectionChangeTime = Time.time + Random.Range(minMoveTime, maxMoveTime);
                return;
            }
        }

        // Если не нашли направление с землёй, пробуем противоположное текущему
        currentDirection = -currentDirection;
        nextDirectionChangeTime = Time.time + Random.Range(minMoveTime, maxMoveTime);
    }

    void DamagePlayers()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, damageRadius, playerLayer);

        foreach (Collider col in cols)
        {
            if (!col.CompareTag("Player"))
                continue;

            GameObject player = col.gameObject;

            if (!damageTimers.ContainsKey(player))
                damageTimers[player] = 0f;

            damageTimers[player] += Time.deltaTime;

            if (damageTimers[player] >= 1f)
            {
                var dmg = player.GetComponent<ITakeDamage>();
                if (dmg != null)
                    dmg.TakeDamage(damagePerSecond);

                damageTimers[player] = 0f;
            }
        }
    }

    void ClearDamageTimers()
    {
        List<GameObject> remove = new List<GameObject>();

        foreach (var pair in damageTimers)
        {
            if (pair.Key == null ||
                Vector3.Distance(pair.Key.transform.position, transform.position) > damageRadius)
                remove.Add(pair.Key);
        }

        foreach (var obj in remove)
            damageTimers.Remove(obj);
    }

    void LiftPlayers()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, liftRadius, playerLayer);

        foreach (Collider col in cols)
        {
            Rigidbody rb = col.attachedRigidbody;
            if (rb == null) continue;

            Vector3 toCenter = transform.position - rb.position;
            toCenter.y = 0;

            float distance = toCenter.magnitude;
            toCenter.Normalize();

            float factor = Mathf.Clamp01(1f - distance / liftRadius);
            bool canLift = rb.position.y < transform.position.y + maxLiftHeight;

            Vector3 force = toCenter * pullForce * factor;

            if (canLift)
                force += Vector3.up * liftForce * factor;

            rb.AddForce(force, ForceMode.Acceleration);
        }
    }

    void DisablePlayersInRadius()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, disableRadius, playerLayer);

        foreach (Collider col in cols)
        {
            GameObject player = col.attachedRigidbody != null
                ? col.attachedRigidbody.gameObject
                : col.gameObject;

            if (disabledPlayers.Contains(player))
                continue;

            disabledPlayers.Add(player);
            StartCoroutine(DisableRoutine(player));
        }
    }

    IEnumerator DisableRoutine(GameObject player)
    {
        NavMeshAgent agent = player.GetComponent<NavMeshAgent>();
        RobloxStyleController controller = player.GetComponent<RobloxStyleController>();

        if (agent != null)
            agent.enabled = false;

        if (controller != null)
            controller.PlayerDisabled();

        yield return new WaitForSeconds(disableTime);

        if (agent != null)
            agent.enabled = true;

        if (controller != null)
            controller.PlayerEnabled();

        disabledPlayers.Remove(player);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, liftRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, damageRadius);
        
        // Визуализация проверки земли
        Gizmos.color = Color.green;
        if (currentDirection != Vector3.zero)
        {
            Gizmos.DrawLine(transform.position, transform.position + currentDirection * groundCheckDistance);
        }
    }
}