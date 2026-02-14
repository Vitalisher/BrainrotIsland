using UnityEngine;
using UnityEngine.UI;

public class ClickerEvent : MonoBehaviour
{
    [Header("Settings")]
    public float timeLimit = 50f;
    public int[] possibleTargets = { 200, 250, 300 };

    [Header("UI")]
    public Text counterText;

    [Header("Player")]
    public Health playerHealth;

    private int targetClicks;
    private int currentClicks;
    private float timer;
    private bool active;

    void Start()
    {
        StartEvent();
    }

    void Update()
    {
        if (!active) return;

        timer -= Time.deltaTime;

        if (Input.GetMouseButtonDown(0))
            currentClicks++;

        UpdateUI();

        if (currentClicks >= targetClicks)
        {
            EndEventSuccess();
            return;
        }

        if (timer <= 0f)
        {
            EndEventFail();
        }
    }

    void StartEvent()
    {
        targetClicks = possibleTargets[Random.Range(0, possibleTargets.Length)];
        currentClicks = 0;
        timer = timeLimit;
        active = true;

        UpdateUI();
    }

    void UpdateUI()
    {
        if (counterText == null) return;

        counterText.text =
            "Клики: " + currentClicks + " / " + targetClicks +
            "\nВремя: " + Mathf.CeilToInt(timer);
    }

    void EndEventSuccess()
    {
        active = false;
        counterText.text = "УСПЕХ!";
        Destroy(gameObject, 1.5f);
    }

    void EndEventFail()
    {
        active = false;
        counterText.text = "ПРОВАЛ!";
        playerHealth.Die();
        Destroy(gameObject, 1.5f);
    }
}