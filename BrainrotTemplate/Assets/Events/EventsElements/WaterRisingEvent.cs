using System;
using UnityEngine;

[CreateAssetMenu(fileName = "WaterRising", menuName = "WorldEvents/WaterRising")]
public class WaterRisingEvent : WorldEvent
{
    public bool rising;
    public bool lowering;

    float timer;
    bool active;

    public override void StartEvent()
    {
        active = true;
        rising = true;
        timer = 0;
        EventUpdater.Instance.Register(UpdateEvent);
    }

    void UpdateEvent()
    {
        if (!active) return;
    }

    public override void EndEvent()
    {
        active = false;
        rising = false; 
        lowering = true;
        EventUpdater.Instance.Unregister(UpdateEvent);
    }
}
