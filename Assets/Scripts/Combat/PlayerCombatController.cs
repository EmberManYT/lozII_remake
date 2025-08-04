using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerCombatController : MonoBehaviour
{
    private Animator animator;
    private PlayerControls playerControls;
    private PlayerControls.PlayerActions playerActions;
    private SideScrollerMovement movement;
    private BoxCollider2D playerCollider;
    private BoxCollider2D swordCollider;
    private PlayerHealth playerHealth;

    [Header("Combat Settings")]
    [SerializeField] private GameObject swordHitbox;
    [SerializeField] private float attackCooldown = 0.1f;
    [SerializeField] private GameObject swordBeamPrefab;
    [SerializeField] private Transform swordHitboxTransform;

    private float lastAttackTime;

    private static readonly int attackTrigger = Animator.StringToHash("attack");
    private static readonly int attackType = Animator.StringToHash("attackType");
    private static readonly int attackHold = Animator.StringToHash("attackHold");

    private bool isWindingUp = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        movement = GetComponent<SideScrollerMovement>();
        playerCollider = GetComponent<BoxCollider2D>();
        swordCollider = swordHitbox.GetComponent<BoxCollider2D>();
        playerHealth = GetComponent<PlayerHealth>();

        swordHitbox.SetActive(false); // Ensure hitbox starts off

        playerControls = new PlayerControls();
        playerActions = playerControls.Player;

        playerActions.Attack.started += OnAttackStarted;
        playerActions.Attack.canceled += OnAttackReleased;
        playerActions.SpecialAttack.started += OnSpecialAttack;
    }

    private void OnEnable() => playerActions.Enable();
    private void OnDisable() => playerActions.Disable();

    private void OnAttackStarted(InputAction.CallbackContext context)
    {
        if (movement.isAttacking || Time.time < lastAttackTime + attackCooldown)
            return;

        int type = DetermineAttackType(false);

        if (type == 0) // Forward standing gets wind-up
        {
            if (isWindingUp) return;

            isWindingUp = true;
            movement.SetAttacking(true);
            animator.SetBool("isWalking", false);
            animator.SetInteger(attackType, 0);
            animator.SetTrigger(attackTrigger);
            animator.SetBool(attackHold, true);
        }
        else
        {
            StartCoroutine(PerformAttackNextFrame(type));
        }
    }

    private void OnAttackReleased(InputAction.CallbackContext context)
    {
        if (!isWindingUp) return;

        isWindingUp = false;
        animator.SetBool(attackHold, false);
        lastAttackTime = Time.time;
    }

    private void OnSpecialAttack(InputAction.CallbackContext context)
    {
        if (movement.isAttacking || Time.time < lastAttackTime + attackCooldown) return;

        int type = DetermineAttackType(true);
        if (type == -1) return;

        StartCoroutine(PerformAttackNextFrame(type));
    }

    private IEnumerator PerformAttackNextFrame(int type)
    {
        movement.SetAttacking(true);
        animator.SetBool("isWalking", false);
        animator.SetInteger(attackType, type);

        yield return null; // Wait one frame to ensure animator sees updated attackType

        animator.SetTrigger(attackTrigger);
        lastAttackTime = Time.time;
    }

    private int DetermineAttackType(bool special)
    {
        bool isCrouched = movement.isCrouched;
        bool isAirborne = !movement.isGrounded;
        float verticalInput = movement.MoveInput.y;
        float verticalVelocity = movement.RB.linearVelocity.y;

        if (special)
        {
            if (isCrouched) return 2;
            if (verticalInput > 0.5f || (isAirborne && verticalVelocity > 0.1f)) return 2;
            if (verticalInput < -0.5f || (isAirborne && verticalVelocity < -0.1f)) return 3;
            return -1;
        }

        return isCrouched ? 1 : 0;
    }

    public void EnableSwordHitbox()
    {
        if (swordHitbox.activeSelf) return;

        swordHitbox.SetActive(true);
        swordCollider.isTrigger = true;

        int type = animator.GetInteger(attackType);
        float direction = movement.MoveInput.x != 0 ? Mathf.Sign(movement.MoveInput.x) : transform.localScale.x >= 0 ? 1f : -1f;

        switch (type)
        {
            case 0:
                swordCollider.offset = new Vector2(8f, -7.5f);
                swordCollider.size = new Vector2(8.5f, 3.5f);
                break;
            case 1:
                swordCollider.offset = new Vector2(6.5f, -13.5f);
                swordCollider.size = new Vector2(9.5f, 4.5f);
                break;
            case 2: //up
                swordCollider.offset = new Vector2(3.5f, -2f);
                swordCollider.size = new Vector2(3.5f, 7.5f);
                break;
            case 3:
                swordCollider.offset = new Vector2(0.5f, -17.5f);
                swordCollider.size = new Vector2(4f, 6.5f);
                break;
        }
        TryFireSwordBeam(type);
    }

    public void DisableSwordHitbox()
    {
        if (!swordHitbox.activeSelf) return;

        swordHitbox.SetActive(false);
        isWindingUp = false;
        animator.SetBool(attackHold, false);
        movement.SetAttacking(false);
        movement.UpdateColliderForState();
    }

    private void TryFireSwordBeam(int attackType)
    {
        if (SwordBeam.Exists) return;
        if (playerHealth.GetCurrentHealth() != playerHealth.GetMaxHealth()) return;

        Vector3 spawnPos = swordCollider != null
            ? swordCollider.bounds.center
            : transform.position;

        GameObject beam = Instantiate(swordBeamPrefab, spawnPos, Quaternion.identity);

        Vector2 dir = Vector2.right;

        switch (attackType)
        {
            case 0:
            case 1:
                dir = transform.lossyScale.x >= 0 ? Vector2.right : Vector2.left;
                break;
            case 2:
                dir = Vector2.up;
                break;
            case 3:
                dir = Vector2.down;
                break;
        }

        beam.GetComponent<SwordBeam>().Launch(dir);
    }

    public void TakeDamage(int amount)
    {
        playerHealth?.TakeDamage(amount);
    }
}
