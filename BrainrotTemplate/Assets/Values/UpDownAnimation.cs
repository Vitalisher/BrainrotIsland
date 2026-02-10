using UnityEngine;

public class UpDownAnimation : MonoBehaviour
{
    public float floatAmplitude = 0.5f; // Амплитуда "плавания" (насколько высоко поднимается)
    public float floatSpeed = 1f;       // Скорость "плавания"

    private float startY; // Начальная локальная позиция по Y

    private void Start()
    {
        startY = transform.localPosition.y;
    }

    private void Update()
    {
        // Движение вверх-вниз относительно родителя
        float newY = startY + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.localPosition = new Vector3(
            transform.localPosition.x,
            newY,
            transform.localPosition.z
        );
    }
}
