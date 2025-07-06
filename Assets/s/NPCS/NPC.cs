using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New NPC", menuName = "NPC/BaseNPC")]
public class NPC : ScriptableObject
{
    public string npcName;
    public int health;

    [SerializeField]
    private float _moveSpeed;

    public float moveSpeed
    {
        get => _moveSpeed;
        set
        {
            if (!Mathf.Approximately(_moveSpeed, value))
            {
                _moveSpeed = value;
                OnMoveSpeedChanged?.Invoke(_moveSpeed);
            }
        }
    }

    // Event triggered when moveSpeed is changed
    public event Action<float> OnMoveSpeedChanged;

    public virtual void Act()
    {
        Debug.Log($"{npcName} is acting.");
    }
}
