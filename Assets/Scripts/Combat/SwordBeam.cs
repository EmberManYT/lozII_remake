using UnityEngine;

public class SwordBeam : MonoBehaviour
{
    public static SwordBeam Instance { get; private set; }

    [SerializeField] private float speed = 12f;
    [SerializeField] private float maxDistance = 10f;

    private Vector2 direction;
    private Vector2 startPos;
    private bool isActive = false;

    public static void InitializeInstance(GameObject prefab)
    {
        if (Instance != null) return;

        GameObject obj = Instantiate(prefab);
        Instance = obj.GetComponent<SwordBeam>();
        obj.SetActive(false);
        DontDestroyOnLoad(obj);
    }

    public void Launch(Vector2 spawnPosition, Vector2 dir)
    {
        transform.position = spawnPosition;
        direction = dir.normalized;
        startPos = spawnPosition;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        gameObject.SetActive(true);
        isActive = true;
    }

    private void Update()
    {
        if (!isActive) return;

        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        if (Vector2.Distance(startPos, transform.position) >= maxDistance)
        {
            Deactivate();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Deactivate();
    }

    public void Deactivate()
    {
        isActive = false;
        gameObject.SetActive(false);
    }
}
