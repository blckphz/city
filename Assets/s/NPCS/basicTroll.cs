using UnityEngine;

public class basicTroll : MonoBehaviour
{
    public NPC trollData;

    void Start()
    {
        if (trollData != null)
        {
            Debug.Log($"Spawned: {trollData.npcName}");
        }
    }

    void Update()
    {
        if (trollData != null)
        {
            trollData.Act(); // Call ScriptableObject logic
        }
    }
}
