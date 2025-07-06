using UnityEngine;
using Pathfinding;

public class trollbrain : MonoBehaviour
{
    private AIDestinationSetter destinationSetter;
    private AIPath aiPath;

    private BuildingStats currentBuildingStats;
    private Transform currentTarget;

    // The building the troll is currently working at (null if none)
    public GameObject isWorkingAt { get; private set; }

    void Start()
    {
        destinationSetter = GetComponent<AIDestinationSetter>();
        aiPath = GetComponent<AIPath>();
    }

    void Update()
    {
        // If assigned to a building but no current target or target changed, reset target to assigned building
        if (isWorkingAt != null && (currentTarget == null || currentTarget.gameObject != isWorkingAt))
        {
            currentTarget = isWorkingAt.transform;
            destinationSetter.target = currentTarget;
            aiPath.canMove = true;
        }

        if (currentTarget != null)
        {
            float distance = Vector3.Distance(transform.position, currentTarget.position);
            if (distance < 1f && currentBuildingStats != null)
            {
                BuildLogic buildLogic = FindObjectOfType<BuildLogic>();
                if (buildLogic != null)
                {
                    buildLogic.FinishBuilding(currentBuildingStats.gameObject);
                }

                // Construction is finished, but DO NOT assign as worker or set parent here.
                // Simply clear the current task.
                ClearWorkingAt();

                currentTarget = null;
                currentBuildingStats = null;
            }
        }
    }


    // Returns true if the troll is currently working somewhere
    public bool IsBusy()
    {
        return isWorkingAt != null;
    }

    public void GoToBuilding(GameObject building)
    {
        // Prevent going to build if already working somewhere
        if (IsBusy())
        {
            Debug.Log($"{gameObject.name} is already working at {isWorkingAt.name} and cannot start building another.");
            return;
        }

        UnregisterFromBuilding();

        currentBuildingStats = building.GetComponent<BuildingStats>();
        if (currentBuildingStats == null)
        {
            Debug.LogWarning("GoToBuilding called with object that has no BuildingStats!");
            return;
        }

        currentTarget = building.transform;
        destinationSetter.target = currentTarget;
        aiPath.canMove = true;

        // Assign working status here
        isWorkingAt = building;
    }

    public void MoveToPosition(Vector3 position)
    {
        UnregisterFromBuilding();

        currentBuildingStats = null;
        currentTarget = null;

        destinationSetter.target = null;
        aiPath.destination = position;
        aiPath.canMove = true;

        // Clear working status when moving somewhere else
        ClearWorkingAt();
    }

    public void AssignAsWorker(GameObject building)
    {
        Debug.Log($"{gameObject.name} assigned to work at {building.name}");

        isWorkingAt = building;

        currentTarget = building.transform;
        destinationSetter.target = currentTarget;
        aiPath.canMove = true;

        // ✅ Parent the troll under the building in the hierarchy
        transform.SetParent(building.transform);
    }


    private void UnregisterFromBuilding()
    {
        if (currentBuildingStats != null && currentBuildingStats.currentlyWorkingHere.Contains(gameObject))
        {
            currentBuildingStats.currentlyWorkingHere.Remove(gameObject);
            transform.SetParent(null);
        }

        ClearWorkingAt();
    }

    private void ClearWorkingAt()
    {
        isWorkingAt = null;
    }

    // Static helper to check if a troll is busy
    public static bool IsTrollWorking(GameObject troll)
    {
        trollbrain brain = troll.GetComponent<trollbrain>();
        if (brain == null)
            return false;

        return brain.IsBusy();
    }
}
