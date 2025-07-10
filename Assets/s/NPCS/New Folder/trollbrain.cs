using System.Collections;
using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(AIPath), typeof(AIDestinationSetter))]
public class trollbrain : MonoBehaviour
{
    public GameObject isWorkingAt = null;

    private AIPath aiPath;
    private AIDestinationSetter destinationSetter;
    private Coroutine currentAction;

    // Distance at which troll is considered "arrived" at building
    public float arriveDistance = 0.5f;

    void Awake()
    {
        aiPath = GetComponent<AIPath>();
        destinationSetter = GetComponent<AIDestinationSetter>();
    }

    // Call this to assign troll to build a planned building
    public void GoToBuilding(GameObject targetBuilding)
    {
        // Clear any previous work assignment first
        if (isWorkingAt != null)
            ClearWorkingAt();

        isWorkingAt = targetBuilding;

        // Stop any ongoing action coroutine
        if (currentAction != null)
            StopCoroutine(currentAction);

        aiPath.canMove = true;   // Enable movement

        currentAction = StartCoroutine(GoAndBuildRoutine(targetBuilding));
    }

    public bool IsBusy()
    {
        // Busy if assigned to a building and/or running an action coroutine
        return isWorkingAt != null || currentAction != null;
    }


    // Coroutine that moves to building and simulates building over time
    private IEnumerator GoAndBuildRoutine(GameObject targetBuilding)
    {
        if (targetBuilding == null)
            yield break;

        Transform buildingTransform = targetBuilding.transform;

        // Set target to move toward
        destinationSetter.target = buildingTransform;

        // Wait until within arriveDistance
        while (Vector3.Distance(transform.position, buildingTransform.position) > arriveDistance)
        {
            yield return null;
        }

        // Arrived - stop moving
        destinationSetter.target = null;
        aiPath.canMove = false;

        Debug.Log($"{gameObject.name} arrived at {targetBuilding.name} and starts building.");

        // Get BuildingStats to know build time
        BuildingStats buildingStats = targetBuilding.GetComponent<BuildingStats>();
        if (buildingStats == null)
        {
            Debug.LogWarning("Target building missing BuildingStats component.");
            yield break;
        }

        if (buildingStats.building == null)
        {
            Debug.LogWarning("BuildingStats missing BaseBuilding reference.");
            yield break;
        }

        int buildTime = buildingStats.building.buildTime;

        // Simulate building progress (simple wait for buildTime seconds)
        float timer = 0f;
        while (timer < buildTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        // Finish building
        buildingStats.SetBuildState(BuildingStats.BuildState.Built);

        Debug.Log($"{gameObject.name} finished building {targetBuilding.name}!");

        // Optionally, troll can now be assigned as a worker
        AssignAsWorker(targetBuilding);

        // Clear currentAction coroutine handle
        currentAction = null;
    }

    // Assign troll as a worker to a finished building
    public void AssignAsWorker(GameObject building)
    {
        if (isWorkingAt != null && isWorkingAt != building)
            ClearWorkingAt();

        isWorkingAt = building;

        // Add self to building's worker list if not already added
        BuildingStats buildingStats = building.GetComponent<BuildingStats>();
        if (buildingStats != null && !buildingStats.currentlyWorkingHere.Contains(gameObject))
        {
            buildingStats.currentlyWorkingHere.Add(gameObject);
        }

        // TODO: Implement worker behavior like resource gathering, production, etc.
        Debug.Log($"{gameObject.name} assigned as worker at {building.name}.");

        // Optionally enable some idle or working animation here
    }

    // Clear the working assignment, remove from building's worker list, and stop movement
    public void ClearWorkingAt()
    {
        if (isWorkingAt == null)
            return;

        BuildingStats buildingStats = isWorkingAt.GetComponent<BuildingStats>();
        if (buildingStats != null)
        {
            buildingStats.RemoveWorker(gameObject);
        }

        isWorkingAt = null;

        // Stop movement and clear target
        if (destinationSetter != null)
            destinationSetter.target = null;

        if (aiPath != null)
            aiPath.canMove = false;

        // Stop any ongoing build coroutine
        if (currentAction != null)
        {
            StopCoroutine(currentAction);
            currentAction = null;
        }

        Debug.Log($"{gameObject.name} cleared working assignment.");
    }
}
