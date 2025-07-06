using UnityEngine;

public class BuildLogic : MonoBehaviour
{
    public GameObject buildingPrefab;      // Assign prefab in inspector
    public LayerMask blockingLayers;

    private GameObject previewInstance;
    private bool isBuilding = false;
    private bool isPlaced = false;         // NEW: track if building is placed (frozen)

    private Camera mainCamera;
    private float zPos = 0f;

    private BoxCollider2D previewCollider;

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
            Debug.LogError("Main Camera not found!");
    }

    void Update()
    {
        // Only move preview if building is active and not yet placed
        if (isBuilding && previewInstance != null && !isPlaced)
        {
            MovePreviewWithMouse();

            // Cancel placement on right mouse click
            if (Input.GetMouseButtonDown(1)) // Right mouse button
            {
                Debug.Log("Cancelled building placement.");
                CancelBuilding();
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

        previewCollider = previewInstance.GetComponent<BoxCollider2D>();
        if (previewCollider == null)
            Debug.LogWarning("Building prefab missing BoxCollider2D!");

        var stats = previewInstance.GetComponent<BuildingStats>();
        if (stats == null)
            stats = previewInstance.AddComponent<BuildingStats>();

        stats.SetBuildState(BuildingStats.BuildState.planned);

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
        Vector2 pos = previewInstance.transform.position;

        Collider2D overlap = Physics2D.OverlapBox(pos, size, 0f, blockingLayers);
        return overlap != null;
    }

    // Call this when player confirms the placement of the building (e.g. presses "place" button or clicks)
    public bool PlaceBuilding()
    {
        if (!isBuilding || previewInstance == null)
            return false;

        if (CheckOverlap())
        {
            Debug.Log("Cannot place building here - overlapping!");
            return false;
        }

        BuildingStats stats = previewInstance.GetComponent<BuildingStats>();
        if (stats != null)
        {
            stats.SetBuildState(BuildingStats.BuildState.placed);
        }

        isPlaced = true;
        isBuilding = false; // ✅ FIX: Reset this after placement!

        Debug.Log("Building placed, now waiting for workers to build it.");

        return true;
    }


    // Call this to cancel building placement and remove preview
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

    // Call this to mark a building as finished by workers
    public void FinishBuilding(GameObject building)
    {
        BuildingStats stats = building.GetComponent<BuildingStats>();
        if (stats == null)
            return;

        stats.SetBuildState(BuildingStats.BuildState.Built);

        // Optionally do any other logic here after building is done
        Debug.Log($"{building.name} construction finished!");
    }

    // Optional: returns if currently building (preview active)
    public bool IsBuilding() => isBuilding;

    // Optional: returns if building is placed (frozen in world)
    public bool IsPlaced() => isPlaced;

    // Optional: returns reference to current preview instance
    public GameObject GetPreviewInstance() => previewInstance;
}
