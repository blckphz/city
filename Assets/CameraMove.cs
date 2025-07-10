using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public float moveSpeed = 5f;

    // Clamp boundaries
    public float minX = -8f;
    public float maxX = 8f;
    public float minY = -5f;
    public float maxY = 5f;

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal"); // A/D or Left/Right
        float moveY = Input.GetAxisRaw("Vertical");   // W/S or Up/Down

        Vector3 move = new Vector3(moveX, moveY, 0f).normalized;
        transform.position += move * moveSpeed * Time.deltaTime;

        // Clamp position using public floats
        float clampedX = Mathf.Clamp(transform.position.x, minX, maxX);
        float clampedY = Mathf.Clamp(transform.position.y, minY, maxY);
        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }
}
