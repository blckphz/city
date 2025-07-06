using System.Collections.Generic;
using UnityEngine;

public class BuildingStats : MonoBehaviour
{
    public List<GameObject> currentlyWorkingHere = new List<GameObject>();
    public BaseBuilding building;

    private float productionTimer = 0f;
    public int currentGrain = 0;

    public bool isPlaced => currentBuildState == BuildState.Built;

    public enum BuildState
    {
        planned,
        Placed,
        Built
    }

    public BuildState currentBuildState = BuildState.planned;

    public void SetBuildState(BuildState newState)
    {
        currentBuildState = newState;
        Debug.Log($"Building '{gameObject.name}' state changed to {currentBuildState}");
    }

    public void AddWorker(GameObject worker)
    {
        if (!currentlyWorkingHere.Contains(worker))
        {
            currentlyWorkingHere.Add(worker);
            worker.transform.SetParent(transform); // Make worker a child immediately when added
            Debug.Log($"{worker.name} added to {gameObject.name}. Total workers: {currentlyWorkingHere.Count}");
        }
        else
        {
            Debug.Log($"{worker.name} is already working at {gameObject.name}");
        }
    }

    // Call this to ensure all workers in the list are children of this building
    public void FixWorkersParent()
    {
        foreach (var worker in currentlyWorkingHere)
        {
            if (worker != null && worker.transform.parent != transform)
            {
                worker.transform.SetParent(transform);
                Debug.Log($"{worker.name} is now a child of {gameObject.name}");
            }
        }
    }

    void Update()
    {
        // Optional: Handle production if this is a mill and is built
        /*
        if (building.buildingType == BuildingType.Mill && currentBuildState == BuildState.Built)
        {
            productionTimer += Time.deltaTime;
            if (productionTimer >= 5f)
            {
                currentGrain++;
                productionTimer = 0f;
                Debug.Log($"Produced grain. Total: {currentGrain}");
            }
        }
        */
    }
}
