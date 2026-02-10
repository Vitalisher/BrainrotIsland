using UnityEngine;

[CreateAssetMenu(fileName = "MeteorShower", menuName = "WorldEvents/MeteorShower")]
public class MeteorShowerEvent : WorldEvent
{
    public GameObject meteorPrefab;
    public float spawnRate = 1f;

    float timer;
    bool active;

    public override void StartEvent()
    {
        active = true;
        timer = 0;
        EventUpdater.Instance.Register(UpdateEvent);
    }

    void UpdateEvent()
    {
        if (!active) return;

        timer += Time.deltaTime;
        if (timer >= 1f / spawnRate)
        {
            timer = 0;
            GameObject.Instantiate(meteorPrefab, meteorPrefab.transform.position, Quaternion.identity);
        }
    }

    public override void EndEvent()
    {
        active = false;
        EventUpdater.Instance.Unregister(UpdateEvent);
    }
}
