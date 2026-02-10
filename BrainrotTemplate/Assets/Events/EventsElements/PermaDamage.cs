using UnityEngine;

public class PermaDamage : MonoBehaviour
{
    public int damage = 10;
    public float radius = 3f;
    public LayerMask playerLayer;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<ITakeDamage>();
            player.TakeDamage(damage);
        }
    }
}