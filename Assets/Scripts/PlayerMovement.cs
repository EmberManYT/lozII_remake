using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float stepSize = 0.5f;       // half tile in one step
    public float stepDuration = 0.35f;  // time to move one step
    public float pixelsPerUnit = 16f;

    private Vector2 input;
    private bool isMoving = false;

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
        if (!isMoving && input != Vector2.zero)
        {
            if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
                input = new Vector2(Mathf.Sign(input.x), 0);
            else
                input = new Vector2(0, Mathf.Sign(input.y));
            StartCoroutine(MoveStep(input));
        }
    }

    IEnumerator MoveStep(Vector2 direction)
    {
        isMoving = true;

        animator.SetFloat("horizontal", direction.x);
        animator.SetFloat("vertical", direction.y);
        animator.SetFloat("speed", 1f);

        Vector2 start = rb.position;
        Vector2 end = start + direction * stepSize;
        float elapsed = 0f;

        while (elapsed < stepDuration)
        {
            Vector2 newPosition = Vector2.Lerp(start, end, elapsed / stepDuration);
            rb.MovePosition(newPosition);
            elapsed += Time.deltaTime;
            yield return null;
        }

        rb.MovePosition(end);
        animator.SetFloat("speed", 0f);
        isMoving = false;
    }
}
