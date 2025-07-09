using UnityEngine;

public class SideScrollerCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Scene X Clamp")]
    public float minX = 26f;
    public float maxX = 74f;

    [Header("Viewport")]
    public float bottomY = 2f;

    [Header("Camera Follow")]
    public float smoothSpeed = 5f;
    private Vector3 velocity = Vector3.zero;
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
        float targetX = target.position.x;  // camera center

        // clamp edges
        float minCamX = minX + halfWidth;
        float maxCamX = maxX - halfWidth;
        float clampedX = Mathf.Clamp(targetX, minCamX, maxCamX);

        Vector3 desiredPosition = new Vector3(clampedX, fixedY, transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, 1f / smoothSpeed);
    }
}
