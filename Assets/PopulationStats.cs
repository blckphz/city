using UnityEngine;
using System.Collections.Generic;

public class PopulationStats : MonoBehaviour
{
    [Header("Population Info")]
    [SerializeField] private int totalPopulation;
    [SerializeField] private int unemployed;
    [SerializeField] private int farmers;
    [SerializeField] private int builders;

    [Header("All Trolls (Auto-populated)")]
    [SerializeField] private List<GameObject> allTrolls = new List<GameObject>();

    void Start()
    {
        RefreshTrollList();
    }

    void Update()
    {
        UpdatePopulationStats();
        AutoAssignBuilders(); // ✅ Continually try to assign builders
    }

    public void RefreshTrollList()
    {
        allTrolls.Clear();
        GameObject[] found = GameObject.FindGameObjectsWithTag("Troop");
        allTrolls.AddRange(found);
    }

    private void UpdatePopulationStats()
    {
        totalPopulation = allTrolls.Count;
        unemployed = 0;
        farmers = 0;
        builders = 0;

        foreach (GameObject troll in allTrolls)
        {
            var brain = troll.GetComponent<trollbrain>();
            if (brain == null)
                continue;

            if (brain.isWorkingAt == null)
            {
                unemployed++;
                continue;
            }

            GameObject building = brain.isWorkingAt;
            BuildingStats buildingStats = building.GetComponent<BuildingStats>();
            BaseBuilding baseBuilding = building.GetComponent<BaseBuilding>();

            if (buildingStats != null &&
                buildingStats.currentBuildState == BuildingStats.BuildState.placed) // ✅ only count as builder if it's placed
            {
                builders++;
            }
            else if (baseBuilding != null && baseBuilding.buildingName == "Farm")
            {
                farmers++;
            }
        }
    }

    // ✅ Auto assign idle trolls to any "placed" building
    private void AutoAssignBuilders()
    {
        List<GameObject> availableTrolls = new List<GameObject>();
        List<GameObject> buildingsNeedingWork = new List<GameObject>();

        // 1. Get available trolls
        foreach (GameObject troll in allTrolls)
        {
            trollbrain brain = troll.GetComponent<trollbrain>();
            if (brain != null && !brain.IsBusy())
            {
                availableTrolls.Add(troll);
            }
        }

        // 2. Get buildings in "placed" state (not yet built)
        BuildingStats[] allBuildings = FindObjectsOfType<BuildingStats>();
        foreach (BuildingStats building in allBuildings)
        {
            if (building.currentBuildState == BuildingStats.BuildState.placed &&
                building.currentlyWorkingHere.Count < 1) // you can increase this to allow multiple builders
            {
                buildingsNeedingWork.Add(building.gameObject);
            }
        }

        // 3. Assign trolls to buildings
        foreach (GameObject building in buildingsNeedingWork)
        {
            if (availableTrolls.Count == 0)
                break;

            GameObject troll = availableTrolls[0];
            availableTrolls.RemoveAt(0);

            trollbrain brain = troll.GetComponent<trollbrain>();
            if (brain != null)
            {
                brain.GoToBuilding(building);
                Debug.Log($"{troll.name} automatically assigned to build {building.name}");
            }
        }
    }

    // External Getters
    public int GetUnemployedCount() => unemployed;
    public int GetFarmerCount() => farmers;
    public int GetBuilderCount() => builders;
    public int GetTotalPopulation() => totalPopulation;
}
