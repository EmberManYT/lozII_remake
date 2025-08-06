using UnityEngine;

public class hitbox : MonoBehaviour
{
    public int damage = 1;
    public bool isPlayerAttack = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Prevent player attacks from affecting shields
        if (isPlayerAttack && other.gameObject.layer == LayerMask.NameToLayer("Shield"))
        {
            Debug.Log("🛡️ Attack blocked by shield!");
            return;
        }

        Transform root = other.transform.root;

        if (isPlayerAttack)
        {
            EnemyHealth enemy = root.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                Debug.Log($"[Hitbox] Damaging enemy: {enemy.name}");
                enemy.TakeDamage(damage);
            }
        }
        else
        {
            PlayerHealth player = root.GetComponent<PlayerHealth>();
            if (player != null)
            {
                Debug.Log($"[Hitbox] Damaging player: {player.name}");
                player.TakeDamage(damage);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = isPlayerAttack ? Color.green : Color.red;

        BoxCollider2D box = GetComponent<BoxCollider2D>();
        if (box != null)
        {
            Vector2 center = (Vector2)transform.position + box.offset;
            Vector2 size = box.size;

            Gizmos.DrawWireCube(center, size);
        }
    }

}
