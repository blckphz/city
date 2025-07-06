using UnityEngine;

public abstract class BaseBuilding : ScriptableObject
{
    public string buildingName;
    public int capacity;
    public Sprite icon;

    // Abstract method to produce resources based on workers count
    public abstract void ProduceResources(int workersCount);
}
