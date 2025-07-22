using UnityEngine;

public class OverworldCamera : MonoBehaviour
{
    public Transform target;
    public float screenWidth = 16f;
    public float screenHeight = 15f;
    public float boundaryLeft = -34f;
    public float boundaryTop = 34f;
    public float boundaryRight = 161f;
    public float boundaryBottom = -99f;
    // 256x240 @ 16 ppu
    public Vector2 cameraOffset = new Vector2(-8f, -5f); // initial alignment for pond (will change)
    private float xMin, xMax, yMin, yMax;

    void Start()
    {
        float centerX = -2.5f; // center around pond (may change)
        float centerY = 2.5f;

        xMin = Mathf.Floor(centerX - screenWidth / 2f) + 0.0f;
        yMin = Mathf.Floor(centerY - screenHeight / 2f) + 0.0f;
        xMax = xMin + screenWidth;
        yMax = yMin + screenHeight;

        xMin = Mathf.Clamp(xMin, boundaryLeft, boundaryRight - screenWidth); // clamp around boundary
        yMin = Mathf.Clamp(yMin, boundaryBottom, boundaryTop - screenHeight);
        xMax = xMin + screenWidth;
        yMax = yMin + screenHeight;

        UpdateCameraPosition();
    }

    void LateUpdate()
    {
        if (target == null) return;
        bool panned = false;

        if (target.position.x >= xMax) // right
        {
            xMin = xMax - 1f;
            xMax = xMin + screenWidth;
            panned = true;
        }
        else if (target.position.x < xMin) // left
        {
            xMax = xMin + 1f;
            xMin = xMax - screenWidth;
            panned = true;
        }

        if (target.position.y >= yMax) // up
        {
            yMin = yMax - 1f;
            yMax = yMin + screenHeight;
            panned = true;
        }
        else if (target.position.y < yMin) // down
        {
            yMax = yMin + 1f;
            yMin = yMax - screenHeight;
            panned = true;
        }

        xMin = Mathf.Clamp(xMin, boundaryLeft, boundaryRight - screenWidth);
        yMin = Mathf.Clamp(yMin, boundaryBottom, boundaryTop - screenHeight);
        xMax = xMin + screenWidth;
        yMax = yMin + screenHeight;

        if (panned) UpdateCameraPosition();
    }

    void UpdateCameraPosition()
    {
        float camX = (xMin + xMax) / 2f;
        float camY = (yMin + yMax) / 2f;
        transform.position = new Vector3(camX, camY, transform.position.z);
        // Debug.Log("Camera position: " + transform.position);
    }
}
