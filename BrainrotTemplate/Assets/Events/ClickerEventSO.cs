using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(
    fileName = "ClickerEvent",
    menuName = "WorldEvents/ClickerEvent"
)]
public class ClickerEventSO : WorldEvent
{
    [Header("Clicker Settings")]
    public float timeLimit = 50f;
    public int[] possibleTargets = { 200, 250, 300 };

    [Header("UI")]
    public Text counterText;

    private int targetClicks;
    private int currentClicks;
    private float timer;
    private bool active;

    public override void StartEvent()
    {
        targetClicks = possibleTargets[Random.Range(0, possibleTargets.Length)];
        currentClicks = 0;
        timer = timeLimit;
        active = true;

        UpdateUI();

        EventUpdater.Instance.Register(UpdateEvent);
    }

    void UpdateEvent()
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
        EventUpdater.Instance.Unregister(UpdateEvent);

        if (counterText != null)
            counterText.text = "УСПЕХ!";
    }

    void EndEventFail()
    {
        active = false;
        EventUpdater.Instance.Unregister(UpdateEvent);

        if (counterText != null)
            counterText.text = "ПРОВАЛ!";

        if (RobloxStyleController.instance != null &&
            RobloxStyleController.instance.health != null)
        {
            var health = RobloxStyleController.instance.health;
            health.TakeDamage(health.MaxHealth);
        }
    }

    public override void EndEvent()
    {
        active = false;
        EventUpdater.Instance.Unregister(UpdateEvent);

        if (counterText != null)
            counterText.text = "";
    }
}
