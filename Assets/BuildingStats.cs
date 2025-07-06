using System.Collections.Generic;
using UnityEngine;

public class BuildingStats : MonoBehaviour
{
    public enum BuildState { planned, placed, Built, Destroyed }

    public BuildState currentBuildState = BuildState.planned;

    public List<GameObject> currentlyWorkingHere = new List<GameObject>();

    private SpriteRenderer spriteRenderer;

    public BaseBuilding building;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            Debug.LogWarning("BuildingStats requires a SpriteRenderer.");
    }

    public void SetBuildState(BuildState state)
    {
        currentBuildState = state;

        switch (state)
        {
            case BuildState.planned:
                if (spriteRenderer != null)
                    spriteRenderer.color = new Color(1f, 1f, 1f, 0.4f); // semi-transparent
                break;
            case BuildState.placed:
                if (spriteRenderer != null)
                    spriteRenderer.color = Color.green;  // green for placed but not built
                break;
            case BuildState.Built:
                if (spriteRenderer != null)
                    spriteRenderer.color = Color.white;
                break;
            case BuildState.Destroyed:
                // add destroy logic here
                break;
        }
    }


    public void RemoveWorker(GameObject unit)
    {
        if (currentlyWorkingHere.Contains(unit))
        {
            currentlyWorkingHere.Remove(unit);

            // Unparent the troll from this building
            if (unit.transform.parent == transform)
                unit.transform.SetParent(null);

            // Clear troll's working assignment
            trollbrain troll = unit.GetComponent<trollbrain>();
            if (troll != null && troll.isWorkingAt == gameObject)
            {
                troll.ClearWorkingAt();
            }

            Debug.Log($"{unit.name} was removed from {gameObject.name} and unparented.");
        }
    }

}
