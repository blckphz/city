using UnityEngine;

public class BuildLogic : MonoBehaviour
{
    public GameObject buildingPrefab; // Assign prefab in inspector
    public LayerMask blockingLayers;

    private GameObject previewInstance;
    private bool isBuilding = false;
    private bool isPlaced = false;

    private Camera mainCamera;
    private float zPos = 0f;

    private BoxCollider2D previewCollider;
    private BuildingStats previewStats;

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
            Debug.LogError("Main Camera not found!");
    }

    void Update()
    {
        if (isBuilding && previewInstance != null && !isPlaced)
        {
            MovePreviewWithMouse();

            if (Input.GetMouseButtonDown(1)) // Right click
            {
                Debug.Log("Cancelled building placement.");
                CancelBuilding();
            }

            if (Input.GetMouseButtonDown(0)) // Left click
            {
                bool placed = PlaceBuilding();
                if (placed)
                    Debug.Log("Building successfully placed.");
            }
        }
    }

    public void StartNewBuilding(GameObject prefab)
    {
        if (isBuilding)
        {
            Debug.LogWarning("Already building something!");
            return;
        }

        if (prefab == null)
        {
            Debug.LogWarning("No building prefab assigned!");
            return;
        }

        previewInstance = Instantiate(prefab);
        previewInstance.transform.position = Vector3.zero;
        previewInstance.transform.rotation = Quaternion.identity;

        previewCollider = previewInstance.GetComponentInChildren<BoxCollider2D>();
        if (previewCollider == null)
            Debug.LogWarning("Building prefab missing BoxCollider2D!");

        previewStats = previewInstance.GetComponentInChildren<BuildingStats>();
        if (previewStats == null)
            Debug.LogWarning("Building prefab missing BuildingStats!");

        if (previewStats != null)
        {
            previewStats.SetBuildState(BuildingStats.BuildState.planned);
        }

        isBuilding = true;
        isPlaced = false;
    }

    void MovePreviewWithMouse()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -mainCamera.transform.position.z + zPos;

        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
        worldPos.z = zPos;

        previewInstance.transform.position = worldPos;
    }

    bool CheckOverlap()
    {
        if (previewCollider == null)
            return false;

        Vector2 size = Vector2.Scale(previewCollider.size, previewInstance.transform.lossyScale);
        Vector2 pos = previewCollider.transform.position; // Use collider's own position

        Collider2D overlap = Physics2D.OverlapBox(pos, size, 0f, blockingLayers);
        return overlap != null;
    }

    public bool PlaceBuilding()
    {
        if (!isBuilding || previewInstance == null)
            return false;

        if (CheckOverlap())
        {
            Debug.Log("Cannot place building here - overlapping!");
            return false;
        }

        if (previewStats != null)
        {
            previewStats.SetBuildState(BuildingStats.BuildState.placed);
        }

        PopulationStats pop = FindObjectOfType<PopulationStats>();
        if (pop != null)
        {
            pop.RefreshTrollList();
        }

        isPlaced = true;
        isBuilding = false;

        Debug.Log("Building placed, waiting for workers to build it.");

        return true;
    }

    public void CancelBuilding()
    {
        if (previewInstance != null)
        {
            Destroy(previewInstance);
            previewInstance = null;
        }

        isBuilding = false;
        isPlaced = false;
    }

    public void FinishBuilding(GameObject building)
    {
        if (building == null) return;

        BuildingStats stats = building.GetComponentInChildren<BuildingStats>();
        if (stats == null)
        {
            Debug.LogWarning("Tried to finish building without BuildingStats!");
            return;
        }

        stats.SetBuildState(BuildingStats.BuildState.Built);
        Debug.Log($"{building.name} construction finished!");
    }

    public bool IsBuilding() => isBuilding;
    public bool IsPlaced() => isPlaced;
    public GameObject GetPreviewInstance() => previewInstance;
}
