using UnityEngine;

public class BallLightning : MonoBehaviour
{
    [Header("Target")]
    public LayerMask playerLayer;
    public float searchRadius = 50f;

    private Transform target;

    [Header("Movement")]
    public float moveSpeed = 6f;
    public float followHeight = 1.5f;
    public float smooth = 6f;

    [Header("Growth")]
    public float growInterval = 10f;
    public float growMultiplier = 1.2f;

    private float growTimer;

    void Update()
    {
        FindClosestTarget();
        FollowTarget();
        HandleGrowth();
    }

    void FindClosestTarget()
    {
        Collider[] cols = Physics.OverlapSphere(
            transform.position,
            searchRadius,
            playerLayer
        );

        float minDist = Mathf.Infinity;
        Transform closest = null;

        foreach (Collider col in cols)
        {
            if (!col.CompareTag("Player")) continue;

            float d = Vector3.Distance(transform.position, col.transform.position);
            if (d < minDist)
            {
                minDist = d;
                closest = col.transform;
            }
        }

        target = closest;
    }

    void FollowTarget()
    {
        if (target == null) return;

        Vector3 targetPos = target.position + Vector3.up * followHeight;
        Vector3 dir = targetPos - transform.position;

        Vector3 velocity = dir.normalized * moveSpeed;

        transform.position = Vector3.Lerp(
            transform.position,
            transform.position + velocity,
            smooth * Time.deltaTime
        );
    }

    void HandleGrowth()
    {
        growTimer += Time.deltaTime;

        if (growTimer >= growInterval)
        {
            growTimer = 0f;
            transform.localScale *= growMultiplier;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, searchRadius);
    }
}