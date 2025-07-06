using UnityEngine;

[CreateAssetMenu(fileName = "NewMill", menuName = "Buildings/Mill")]
public class Mill : BaseBuilding
{
    public int grainStorageCapacity;
    public int grainProcessingRate;  // grain produced per worker per cycle

    public override void ProduceResources(int workersCount)
    {
        int totalGrainProduced = workersCount * grainProcessingRate;

        // Example: Just log for now, replace with actual storage or resource manager
        Debug.Log($"Mill produced {totalGrainProduced} grain with {workersCount} workers.");
    }
}
