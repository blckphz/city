using UnityEngine;

public class BuildLogic : MonoBehaviour
{
    public GameObject buildingPrefab;      // Assign prefab in inspector
    public LayerMask blockingLayers;

    private GameObject previewInstance;
    private bool isBuilding = false;

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
        if (isBuilding && previewInstance != null)
        {
            MovePreviewWithMouse();
            // No color updates here, handled elsewhere
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

    public void ConfirmPlacement(GameObject building)
    {
        if (!isBuilding || previewInstance == null)
            return;

        if (CheckOverlap())
        {
            Debug.Log("Cannot place building here - overlapping!");
            return;
        }

        var stats = previewInstance.GetComponent<BuildingStats>();
        if (stats != null)
            stats.SetBuildState(BuildingStats.BuildState.Placed);

        isBuilding = false;  // stop moving preview

        Debug.Log("Building placed but awaiting construction!");
    }

    public void FinishBuilding(GameObject building)
    {
        if (building == null)
            return;

        BuildingStats stats = building.GetComponent<BuildingStats>();
        if (stats != null)
            stats.SetBuildState(BuildingStats.BuildState.Built);

        Debug.Log("Building construction completed!");
    }
}
