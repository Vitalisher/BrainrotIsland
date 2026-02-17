using UnityEngine;
using System.Collections.Generic;

public class Meteor : MonoBehaviour
{
    public GameObject warningCirclePrefab;
    public GameObject explosionEffect;

    [Header("Fall")]
    public float fallSpeed = 15f;
    public float mapRadius = 50f;
    public LayerMask groundMask;

    [Header("Damage")]
    public int damage = 40;
    public float explosionRadiusMin = 4f;
    public float explosionRadiusMax = 7f;
    public float currentExplosion;

    public LayerMask playerLayer;

    Vector3 targetPosition;
    GameObject warningCircle;
    bool isFalling = true;

    void Start()
    {
        currentExplosion = Mathf.Round(Random.Range(explosionRadiusMin, explosionRadiusMax));

        float x = Random.Range(-mapRadius, mapRadius);
        float z = Random.Range(-mapRadius, mapRadius);
        float y = Random.Range(60, 100);

        fallSpeed = Random.Range(35, 48);

        Vector3 rayOrigin = new Vector3(x, y, z);
        RaycastHit hit;

        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, Mathf.Infinity, groundMask))
            targetPosition = hit.point;
        else
            targetPosition = new Vector3(x, 0, z);

        transform.position = new Vector3(targetPosition.x, y + 1, targetPosition.z);

        warningCircle = Instantiate(warningCirclePrefab, targetPosition, Quaternion.identity);
        warningCircle.transform.localScale = new Vector3(currentExplosion * 2, 0.1f, currentExplosion * 2);
    }

    void Update()
    {
        if (!isFalling) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            fallSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetPosition) < 0.5f)
            Impact();
    }

    void Impact()
    {
        isFalling = false;

        // 💥 УРОН ПО ОБЛАСТИ
        DealDamage();

        if (explosionEffect != null)
            Instantiate(explosionEffect, targetPosition, Quaternion.identity);

        if (warningCircle != null)
            Destroy(warningCircle);

        Destroy(gameObject);
    }

    void DealDamage()
    {
        Collider[] hits = Physics.OverlapSphere(
            targetPosition,
            currentExplosion,
            playerLayer
        );

        // Используем HashSet для отслеживания уже повреждённых объектов
        HashSet<GameObject> damagedObjects = new HashSet<GameObject>();

        foreach (Collider col in hits)
        {
            // Получаем корневой GameObject (на случай если коллайдер на дочернем объекте)
            GameObject rootObject = col.attachedRigidbody != null 
                ? col.attachedRigidbody.gameObject 
                : col.gameObject;

            // Пропускаем если уже нанесли урон этому объекту
            if (damagedObjects.Contains(rootObject))
                continue;

            ITakeDamage damageable = rootObject.GetComponent<ITakeDamage>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
                damagedObjects.Add(rootObject);
            }
        }
    }

    // Визуализация радиуса взрыва
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(targetPosition, currentExplosion);
    }
}