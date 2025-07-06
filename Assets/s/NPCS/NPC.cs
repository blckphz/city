using UnityEngine;

[CreateAssetMenu(fileName = "New NPC", menuName = "NPC/BaseNPC")]
public class NPC : ScriptableObject
{
    public string npcName;
    public int health;
    public float moveSpeed;

    public virtual void Act()
    {
        // Base behavior
        Debug.Log($"{npcName} is acting.");
    }
}
