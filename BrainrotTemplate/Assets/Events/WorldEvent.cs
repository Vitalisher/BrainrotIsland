using UnityEngine;

public abstract class WorldEvent : ScriptableObject
{
    public string eventNameRu;
    public string eventNameEng;
    public string eventNameTr;
    public float duration;
    public abstract void StartEvent();
    public abstract void EndEvent();
}
