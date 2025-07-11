using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildingStats : MonoBehaviour
{
    public enum BuildState { planned, placed, Built, Destroyed }

    public BuildState currentBuildState = BuildState.planned;
    public List<GameObject> currentlyWorkingHere = new List<GameObject>();

    [Header("Optional: assign manually or auto-detect")]
    public SpriteRenderer spriteRenderer;
    public TilemapRenderer tilemapRenderer;
    private Tilemap tilemap; // Needed for tinting

    public BaseBuilding building;

    void Awake()
    {
        // Auto-find SpriteRenderer
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        // Auto-find TilemapRenderer and Tilemap
        if (tilemapRenderer == null)
            tilemapRenderer = GetComponentInChildren<TilemapRenderer>();

        if (tilemap == null && tilemapRenderer != null)
            tilemap = tilemapRenderer.GetComponent<Tilemap>();

        // Clone SpriteRenderer's material
        if (spriteRenderer != null)
        {
            Material original = spriteRenderer.sharedMaterial;
            if (original != null)
                spriteRenderer.material = new Material(original);
            else
                spriteRenderer.material = new Material(Shader.Find("Sprites/Default"));
        }

        // Clone TilemapRenderer's material
        if (tilemapRenderer != null)
        {
            Material original = tilemapRenderer.sharedMaterial;
            Material baseMaterial = tilemapRenderer.sharedMaterial != null
       ? tilemapRenderer.sharedMaterial
       : new Material(Shader.Find("Sprites/Default"));

            tilemapRenderer.material = new Material(baseMaterial);

        }

        if (spriteRenderer == null && tilemap == null)
            Debug.LogWarning("BuildingStats requires a SpriteRenderer or Tilemap for visual tinting.");
    }

    public void SetBuildState(BuildState state)
    {
        currentBuildState = state;

        // Apply tint depending on render type
        Color tint = Color.white;

        switch (state)
        {
            case BuildState.planned:
                tint = new Color(0f, 1f, 0f, 0.5f); // semi-transparent green instead of white
                break;
            case BuildState.placed:
                tint = new Color(0f, 1f, 0f, 0.5f); // keep placed green tint same or different if you want
                break;
            case BuildState.Built:
                tint = Color.white;
                break;
            case BuildState.Destroyed:
                tint = Color.red;
                break;
        }


        // Apply tint to sprite or tilemap
        if (spriteRenderer != null)
            spriteRenderer.color = tint;

        if (tilemap != null)
            tilemap.color = tint;

        Debug.Log($"Build state set to {state}. Tint applied.");
    }

    public void RemoveWorker(GameObject unit)
    {
        if (currentlyWorkingHere.Contains(unit))
        {
            currentlyWorkingHere.Remove(unit);

            if (unit.transform.parent == transform)
                unit.transform.SetParent(null);

            trollbrain troll = unit.GetComponent<trollbrain>();
            if (troll != null && troll.isWorkingAt == gameObject)
                troll.ClearWorkingAt();

            Debug.Log($"{unit.name} removed from {gameObject.name} and unparented.");
        }
    }
}
