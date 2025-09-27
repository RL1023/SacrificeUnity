using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float moveSpeedMultiplier = 1f;

    public bool canMove = true;
    public bool canDash = true;
    public float dashDistance = 5f;
    public float dashDuration = 0.15f; // how long the dash lasts
    public float dashCooldown = 1f;   // time before next dash

    public float acceleration = 10f;
    public float deceleration = 15f;

    public Vector3 offset = Vector3.zero;
    public bool flipOnLeft = true;

    private Vector2 inputDirection;
    private Vector2 currentVelocity;
    private Rigidbody2D rb;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private bool isDashing = false;
    private float dashCooldownTimer = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        inputDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        // Flip character based on direction
        if (flipOnLeft && inputDirection.x != 0)
        {
            spriteRenderer.flipX = inputDirection.x < 0;
        }

        // Trigger animation only when actual speed is > 0
        if (animator != null)
        {
            bool isMoving = currentVelocity.magnitude > 0.05f;
            animator.SetBool("isMoving", isMoving);

            bool notMoving = currentVelocity.magnitude == 0.0f;
            animator.SetBool("notMoving", notMoving);
        }

        // Turbo example
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
        {
            turbo();
        }

        // Dash trigger
        if (canDash && dashCooldownTimer <= 0 && Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(Dash());
        }

        if (dashCooldownTimer > 0)
            dashCooldownTimer -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (isDashing) return; // skip normal movement while dashing

        if (inputDirection.magnitude > 0.1f)
        {
            currentVelocity = Vector2.MoveTowards(currentVelocity, inputDirection * moveSpeed, acceleration * Time.fixedDeltaTime);
        }
        else
        {
            currentVelocity = Vector2.MoveTowards(currentVelocity, Vector2.zero, deceleration * Time.fixedDeltaTime);
        }

        rb.linearVelocity = currentVelocity;
    }

    private System.Collections.IEnumerator Dash()
    {
        isDashing = true;
        canDash = false;
        dashCooldownTimer = dashCooldown;

        Vector2 dashDirection = inputDirection != Vector2.zero ? inputDirection : Vector2.right * (spriteRenderer.flipX ? -1 : 1);

        rb.linearVelocity = dashDirection * (dashDistance / dashDuration);

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;
        canDash = true;
    }

    private void turbo()
    {
        // Example: temporarily boost speed
        StartCoroutine(TurboBoost());
    }

    private System.Collections.IEnumerator TurboBoost()
    {
        float originalSpeed = moveSpeed;
        moveSpeed *= 2f;
        yield return new WaitForSeconds(1f);
        moveSpeed = originalSpeed;
    }
}
