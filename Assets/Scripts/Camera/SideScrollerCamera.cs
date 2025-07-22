using UnityEngine;

public class SideScrollerCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Scene X Clamps")]
    public float minX = 26f;
    public float maxX = 74f;

    [Header("Viewport")]
    public float bottomY = 2f;

    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (target == null) return;

        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;
        float fixedY = bottomY + halfHeight;

        float targetCenteredX = target.position.x;
        float minCamX = minX + halfWidth;
        float maxCamX = maxX - halfWidth;

        float desiredCameraX = targetCenteredX;

        if (desiredCameraX < minCamX)
        {
            desiredCameraX = minCamX;
            if (target.position.x > (minCamX + halfWidth * 0.1f) && transform.position.x == minCamX)
            {
                desiredCameraX = targetCenteredX;
            }
        }
        else if (desiredCameraX > maxCamX)
        {
            desiredCameraX = maxCamX;
            if (target.position.x < (maxCamX - halfWidth * 0.1f) && transform.position.x == maxCamX)
            {
                desiredCameraX = targetCenteredX;
            }
        }

        desiredCameraX = Mathf.Clamp(desiredCameraX, minCamX, maxCamX);
        Vector3 desiredPosition = new Vector3(desiredCameraX, fixedY, transform.position.z);
        transform.position = desiredPosition;
    }
}