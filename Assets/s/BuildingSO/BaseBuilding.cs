using UnityEngine;

public abstract class BaseBuilding : ScriptableObject
{
    public string buildingName;
    public int capacity;
    public Sprite icon;
    public int buildTime;  // Build time in ticks (e.g. seconds or custom units)

    // Abstract method to produce resources based on workers count
    public abstract void ProduceResources(int workersCount);
}
