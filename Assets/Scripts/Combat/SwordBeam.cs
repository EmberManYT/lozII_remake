using UnityEngine;

public class SwordBeam : MonoBehaviour
{
    [SerializeField] private float speed = 12f;
    [SerializeField] private float maxDistance = 10f;

    private Vector2 direction;
    private Vector2 startPos;

    public static bool Exists = false;

    public void Launch(Vector2 dir)
    {
        direction = dir.normalized;
        startPos = transform.position;
        Exists = true;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        if (Vector2.Distance(startPos, transform.position) >= maxDistance)
        {
            DestroyBeam();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        DestroyBeam();
    }

    private void OnDestroy()
    {
        Exists = false;
    }

    private void DestroyBeam()
    {
        Destroy(gameObject);
    }
}
