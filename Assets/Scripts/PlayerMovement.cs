using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float stepSize = 0.5f;       // Half tile per step
    public float stepDuration = 0.6f;  // Duration of one step

    private Vector2 input;
    private bool IsWalking = false;

    private PlayerControls controls;
    private Animator animator;
    private Rigidbody2D rb;

    void Awake()
    {
        controls = new PlayerControls();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        controls.Player.Move.performed += ctx => input = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => input = Vector2.zero;
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void Update()
    {
        if (!IsWalking && input != Vector2.zero)
        {
            StartCoroutine(MoveUntilReleased());
        }
    }

    IEnumerator MoveUntilReleased()
    {
        IsWalking = true;

        while (input != Vector2.zero)
        {
            // Choose 4-direction direction
            Vector2 moveDir = input;
            if (Mathf.Abs(moveDir.x) > Mathf.Abs(moveDir.y))
                moveDir = new Vector2(Mathf.Sign(moveDir.x), 0);
            else
                moveDir = new Vector2(0, Mathf.Sign(moveDir.y));

            // Set blend tree direction and trigger walk
            animator.SetFloat("MoveX", moveDir.x);
            animator.SetFloat("MoveY", moveDir.y);
            animator.SetBool("IsWalking", true);

            yield return StartCoroutine(MoveStep(moveDir));

            // Reset to idle pose and pause
            animator.SetBool("IsWalking", false);
            yield return new WaitForSeconds(0.12f);
        }

        IsWalking = false;
    }


    IEnumerator MoveStep(Vector2 direction)
    {
        Vector2 start = rb.position;
        Vector2 end = start + direction * stepSize; // stepSize = 0.25f or 0.5f
        float elapsed = 0f;

        while (elapsed < stepDuration)
        {
            rb.MovePosition(Vector2.Lerp(start, end, elapsed / stepDuration));
            elapsed += Time.deltaTime;
            yield return null;
        }

        rb.MovePosition(end);
    }

}
