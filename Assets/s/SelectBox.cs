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
    /// </summary>
    public void SendSelectedToBuilding(GameObject building)
    {
        Debug.Log($"Sending {selectedUnits.Count} selected units to building {building.name}");
        foreach (GameObject unit in selectedUnits)
        {
            trollbrain brain = unit.GetComponent<trollbrain>();
            BuildingStats buildingStats = building.GetComponent<BuildingStats>();

            if (brain != null && buildingStats != null)
            {
                if (buildingStats.currentBuildState == BuildingStats.BuildState.planned)
                {
                    // If building is not yet built, send units to construct it
                    brain.GoToBuilding(building);
                }
                else if (buildingStats.currentBuildState == BuildingStats.BuildState.Built)
                {
                    // If building is built, assign units as workers
                    brain.AssignAsWorker(building);

                    if (!buildingStats.currentlyWorkingHere.Contains(unit))
                        buildingStats.currentlyWorkingHere.Add(unit);
                }
            }
        }
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
