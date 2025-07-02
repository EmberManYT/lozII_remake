using UnityEngine;

public class OverworldCamera : MonoBehaviour
{
    public Transform target;
    public float screenWidth = 16f;
    public float screenHeight = 15f;
    // 256x240 @ 16 ppu
    public Vector2 cameraOffset = new Vector2(-8.04f, -4.98f); // initial alignment for pond (will change)
    private float xMin, xMax, yMin, yMax;

    void Start()
    {
        if (target == null) return;
        Vector2 start = (Vector2)target.position - cameraOffset;

        xMin = Mathf.Floor(start.x / screenWidth) * screenWidth + cameraOffset.x;
        yMin = Mathf.Floor(start.y / screenHeight) * screenHeight + cameraOffset.y;
        xMax = xMin + screenWidth;
        yMax = yMin + screenHeight;

        UpdateCameraPosition();
    }

    void LateUpdate()
    {
        if (target == null) return;
        Vector2 pos = target.position;

        // right border crossed
        if (pos.x >= xMax)
        {
            xMin = xMax;
            xMax = xMin + screenWidth;

            float centerY = pos.y;
            yMin = centerY - screenHeight / 2f;
            yMax = yMin + screenHeight;

            UpdateCameraPosition();
        }

        // left border crossed
        else if (pos.x < xMin)
        {
            xMax = xMin;
            xMin = xMax - screenWidth;

            float centerY = pos.y;
            yMin = centerY - screenHeight / 2f;
            yMax = yMin + screenHeight;

            UpdateCameraPosition();
        }

        // top border crossed
        else if (pos.y >= yMax)
        {
            yMin = yMax;
            yMax = yMin + screenHeight;

            float centerX = pos.x;
            xMin = centerX - screenWidth / 2f;
            xMax = xMin + screenWidth;

            UpdateCameraPosition();
        }

        // bottom border crossed
        else if (pos.y < yMin)
        {
            yMax = yMin;
            yMin = yMax - screenHeight;

            float centerX = pos.x;
            xMin = centerX - screenWidth / 2f;
            xMax = xMin + screenWidth;

            UpdateCameraPosition();
        }
    }

    void UpdateCameraPosition()
    {
        float camX = (xMin + xMax) / 2f;
        float camY = (yMin + yMax) / 2f;

        transform.position = new Vector3(camX, camY, transform.position.z);
        // Debug.Log("Camera position: " + transform.position);
    }
}
