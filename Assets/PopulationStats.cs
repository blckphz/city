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

            // Check if it's still under construction
            if (buildingStats != null &&
                (buildingStats.currentBuildState == BuildingStats.BuildState.planned ||
                 buildingStats.currentBuildState == BuildingStats.BuildState.placed))
            {
                builders++;
            }
            else if (baseBuilding != null && baseBuilding.buildingName == "Farm")
            {
                farmers++;
            }
            else
            {
                // Could add logic for other job types here
            }
        }
    }

    // Optional: Getters for external use
    public int GetUnemployedCount() => unemployed;
    public int GetFarmerCount() => farmers;
    public int GetBuilderCount() => builders;
    public int GetTotalPopulation() => totalPopulation;
}
