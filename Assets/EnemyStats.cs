using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(AIPath))]
public class EnemyStats : MonoBehaviour
{
    [Header("NPC Data")]
    public NPC thisNpc;

    [Header("Runtime Stats")]
    public float currentHealth;
    public float currentWalkspeed;

    private AIPath aiPath;
    private float lastSetSpeed = -1f; // Use a sentinel value to detect first-time set

    void Awake()
    {
        aiPath = GetComponent<AIPath>();
    }

    void Start()
    {
        if (thisNpc != null)
        {
            currentHealth = thisNpc.health;
            UpdateMovementSpeed(force: true);
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} has no NPC data assigned.");
        }
    }

    void Update()
    {
        UpdateMovementSpeed();
    }

    private void UpdateMovementSpeed(bool force = false)
    {
        if (thisNpc == null || aiPath == null)
            return;

        // Update currentWalkspeed to match NPC data
        if (force || !Mathf.Approximately(thisNpc.moveSpeed, lastSetSpeed))
        {
            currentWalkspeed = thisNpc.moveSpeed;
            lastSetSpeed = thisNpc.moveSpeed;
        }

        // Ensure aiPath.maxSpeed is always equal to currentWalkspeed
        if (!Mathf.Approximately(aiPath.maxSpeed, currentWalkspeed))
        {
            aiPath.maxSpeed = currentWalkspeed;
        }
    }

}
