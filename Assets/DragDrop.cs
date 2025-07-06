using UnityEngine;
using UnityEngine.EventSystems;
using static trollbrain;

public class DragDrop : MonoBehaviour, IPointerClickHandler
{
    private Camera mainCamera;
    private float zPos;
    private BuildingStats buildingStats;
    private Renderer rend;

    public BuildLogic buildLogic;

    private Color greenTransparent = new Color(0f, 1f, 0f, 0.5f);
    private Color whiteOpaque = new Color(1f, 1f, 1f, 1f);

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
            Debug.LogError("Main Camera not found!");

        zPos = transform.position.z;

        buildingStats = GetComponent<BuildingStats>();
        if (buildingStats == null)
            Debug.LogWarning("No BuildingStats component found on this GameObject.");

        if (buildLogic == null)
            buildLogic = FindObjectOfType<BuildLogic>();

        rend = GetComponent<Renderer>();
        if (rend == null)
            Debug.LogWarning("No Renderer component found on this GameObject.");

        UpdateColorByState();
    }

    void Update()
    {
        if (buildingStats != null)
        {
            if (buildingStats.currentBuildState == BuildingStats.BuildState.planned)
            {
                MoveWithMouse();
            }

            UpdateColorByState();
        }
    }

    void MoveWithMouse()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -mainCamera.transform.position.z + zPos;

        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
        worldPos.z = zPos;

        transform.position = worldPos;
    }

    void UpdateColorByState()
    {
        if (rend == null) return;

        if (buildingStats.currentBuildState == BuildingStats.BuildState.Built)
        {
            SetColor(whiteOpaque);
        }
        else
        {
            // planned and placed are green transparent
            SetColor(greenTransparent);
        }
    }

    void SetColor(Color color)
    {
        if (rend.material.HasProperty("_Color"))
        {
            Color currentColor = rend.material.color;
            if (currentColor != color)
                rend.material.color = color;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (buildingStats != null)
        {
            if (buildingStats.currentBuildState == BuildingStats.BuildState.planned)
            {
                if (buildLogic != null)
                {
                    buildLogic.PlaceBuilding();
                    Debug.Log($"Building preview placed at {transform.position}");
                }
                else
                {
                    Debug.LogWarning("No BuildLogic reference in DragDrop!");
                }
            }
            else if (buildingStats.currentBuildState == BuildingStats.BuildState.Built)
            {
                AssignTrollToJob();

                // ✅ New line: Send selected troops to this building
                SelectBox.Instance.SendSelectedToBuilding(gameObject);
            }

        }
    }

    private void AssignTrollToJob()
    {
        trollbrain[] trolls = FindObjectsOfType<trollbrain>();

        foreach (var troll in trolls)
        {
            if (troll.isWorkingAt == null)
            {
                // Assign troll to this building
                troll.AssignAsWorker(gameObject);
                troll.GoToBuilding(gameObject);

                // Make troll child of the built building GameObject
                troll.transform.SetParent(gameObject.transform);

                Debug.Log($"{troll.gameObject.name} assigned to work at {gameObject.name}");
                return;
            }
        }

        Debug.Log("No free trolls available to assign to this building.");
    }
}
