using UnityEngine;

public class Coin : MonoBehaviour
{
    public int count = 1;
    public float rotationSpeed = 100f; // Скорость вращения
    public float floatAmplitude = 0.5f; // Амплитуда "плавания" (насколько высоко поднимается)
    public float floatSpeed = 1f; // Скорость "плавания"

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        // Вращение монетки вокруг оси Y
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // Плавное "плавание" вверх-вниз
        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<RobloxStyleController>())
        {
            PlayerStats.instance.AddCoins(1, "coins");
        }

        Destroy(gameObject);
    }
}
