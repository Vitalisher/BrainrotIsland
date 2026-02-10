using System;
using System.Collections.Generic;
using UnityEngine;

public class EventUpdater : MonoBehaviour
{
    public static EventUpdater Instance;
    List<Action> updates = new();

    void Awake()
    {
        Instance = this;
    }

    public void Register(Action action) => updates.Add(action);
    public void Unregister(Action action) => updates.Remove(action);

    void Update()
    {
        foreach (var a in updates) a?.Invoke();
    }
}
