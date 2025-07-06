using UnityEngine;
using System.Collections.Generic;

public class SelectBox : MonoBehaviour
{
    private Vector2 startPos;
    private Vector2 endPos;
    private bool isDragging = false;
    private Texture2D whiteTex;

    [SerializeField] private List<GameObject> selectedUnits = new List<GameObject>();
    [Header("Optional: Show this when units are selected")]
    [SerializeField] private GameObject selectionIndicator;

    public static SelectBox Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Called from BuildingClickTarget when a building is clicked.
    /// Sends all selected units to the building to start working or constructing.
    /// Removes units from previous building if working elsewhere.
    /// Deselects units after assigning jobs.
    /// </summary>
    public void SendSelectedToBuilding(GameObject building)
    {
        Debug.Log($"Sending {selectedUnits.Count} selected units to building {building.name}");

        List<GameObject> unitsToDeselect = new List<GameObject>();

        BuildingStats targetBuildingStats = building.GetComponent<BuildingStats>();
        if (targetBuildingStats == null)
        {
            Debug.LogWarning("Target building missing BuildingStats component!");
            return;
        }

        foreach (GameObject unit in selectedUnits)
        {
            trollbrain brain = unit.GetComponent<trollbrain>();
            if (brain == null)
            {
                Debug.LogWarning($"{unit.name} missing trollbrain component!");
                continue;
            }

            // Check if the unit is currently working at another building and remove it properly
            BuildingStats currentBuilding = null;
            foreach (var b in FindObjectsOfType<BuildingStats>())
            {
                if (b.currentlyWorkingHere.Contains(unit))
                {
                    currentBuilding = b;
                    break;
                }
            }
            if (currentBuilding != null && currentBuilding != targetBuildingStats)
            {
                currentBuilding.RemoveWorker(unit);  // Properly remove worker
                brain.ClearWorkingAt();              // Clear busy state so troop can be reassigned
                Debug.Log($"{unit.name} removed from previous building {currentBuilding.gameObject.name}");
            }

            // Assign unit to new building depending on its state
            if (targetBuildingStats.currentBuildState == BuildingStats.BuildState.planned)
            {
                // Building not built yet — send unit to construct it
                brain.GoToBuilding(building);
                unitsToDeselect.Add(unit);
            }
            else if (targetBuildingStats.currentBuildState == BuildingStats.BuildState.Built)
            {
                // Building built — assign as worker
                brain.AssignAsWorker(building);

                if (!targetBuildingStats.currentlyWorkingHere.Contains(unit))
                    targetBuildingStats.currentlyWorkingHere.Add(unit);

                unitsToDeselect.Add(unit);
            }
        }

        // Remove assigned units from selection
        foreach (GameObject unit in unitsToDeselect)
        {
            selectedUnits.Remove(unit);
        }

        // Update selection indicator visibility
        if (selectionIndicator != null)
            selectionIndicator.SetActive(selectedUnits.Count > 0);
    }

    void Start()
    {
        whiteTex = Texture2D.whiteTexture;

        if (selectionIndicator != null)
            selectionIndicator.SetActive(false);
    }

    void Update()
    {
        HandleInput();
        UpdateSelectionIndicator();
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(1)) // Right mouse button starts selection drag
        {
            startPos = Input.mousePosition;
            isDragging = true;
        }

        if (Input.GetMouseButtonUp(1)) // Release ends selection drag
        {
            isDragging = false;
            endPos = Input.mousePosition;
            SelectUnitsInArea();
        }
    }

    void SelectUnitsInArea()
    {
        selectedUnits.Clear();

        Vector2 min = Vector2.Min(startPos, endPos);
        Vector2 max = Vector2.Max(startPos, endPos);
        Rect screenRect = new Rect(min, max - min);

        GameObject[] allUnits = GameObject.FindGameObjectsWithTag("Troop");
        Debug.Log($"Selecting units in area. Found {allUnits.Length} troops in scene.");

        foreach (GameObject unit in allUnits)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(unit.transform.position);
            if (screenRect.Contains(screenPos, true))
            {
                selectedUnits.Add(unit);
                Debug.Log($"Selected unit: {unit.name}");
            }
        }

        Debug.Log($"Total units selected: {selectedUnits.Count}");
    }

    void UpdateSelectionIndicator()
    {
        if (selectionIndicator != null)
            selectionIndicator.SetActive(selectedUnits.Count > 0);
    }

    void OnGUI()
    {
        if (isDragging)
        {
            Rect rect = GetScreenRect(startPos, Input.mousePosition);
            DrawScreenRect(rect, new Color(0.3f, 0.8f, 1f, 0.25f));
            DrawScreenRectBorder(rect, 2, new Color(0.3f, 0.8f, 1f));
        }
    }

    Rect GetScreenRect(Vector2 p1, Vector2 p2)
    {
        Vector2 topLeft = Vector2.Min(p1, p2);
        Vector2 bottomRight = Vector2.Max(p1, p2);
        return new Rect(topLeft.x, Screen.height - bottomRight.y, bottomRight.x - topLeft.x, bottomRight.y - topLeft.y);
    }

    void DrawScreenRect(Rect rect, Color color)
    {
        GUI.color = color;
        GUI.DrawTexture(rect, whiteTex);
        GUI.color = Color.white;
    }

    void DrawScreenRectBorder(Rect rect, float thickness, Color color)
    {
        DrawScreenRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color); // Top
        DrawScreenRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color); // Left
        DrawScreenRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color); // Right
        DrawScreenRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color); // Bottom
    }

    public int SelectedUnitsCount() => selectedUnits.Count;
    public List<GameObject> GetSelectedUnits() => selectedUnits;
}
