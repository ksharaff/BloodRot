using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Sounds")]
    public AudioClip dashSound;
    public AudioClip walkSound;
    public AudioClip runSound;
    [Range(0f, 0.3f)] public float pitchVariation;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float runSpeed = 8f;
    public float runStaminaPerSecond = 20f;

    [Header("Jump (Hollow Knight style)")]
    public float jumpVelocity = 14f;        // Takeoff speed: higher = higher jump
    public float jumpCutMultiplier = 0.4f;  // Releasing Space early multiplies upward speed by this
    public float coyoteTime = 0.1f;         // Still allowed to jump this long after leaving a ledge
    public float jumpBufferTime = 0.1f;     // Pressing jump just before landing still counts
    public float riseGravity = 3f;
    public float fallGravity = 5f;          // Heavier falling feels snappier
    public LayerMask groundLayer;           // Set to "Ground" in the Inspector

    [Header("Dash")]
    public float dashSpeed = 16f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 0.5f;
    public float dashStaminaCost = 25f;

    public bool IsRunning { get; private set; }

    private Rigidbody2D rb;
    private BoxCollider2D col;
    private PlayerStamina stamina;
    private PlayerAnimation anim;
    private AudioSource audioSource;

    private float moveInput;
    private float facing = 1f;          // 1 = right, -1 = left
    private bool runLocked = false;     // True after stamina runs out, until Shift is released

    private bool isGrounded, wasGrounded;
    private float coyoteTimer, jumpBufferTimer;
    private bool inJumpAnim = false;    // True from takeoff until we land

    private bool isDashing = false;
    private bool airDashUsed = false;   // One dash per time in the air
    private float dashTimer, dashCooldownTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        stamina = GetComponent<PlayerStamina>();
        anim = GetComponent<PlayerAnimation>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (dashCooldownTimer > 0f) dashCooldownTimer -= Time.deltaTime;

        // Grounded only counts when we're not moving upward (so takeoff doesn't count as grounded)
        wasGrounded = isGrounded;
        isGrounded = CheckGrounded() && rb.linearVelocity.y <= 0.1f;
        if (isGrounded) airDashUsed = false;

        // Just landed: play the landing animation
        if (!wasGrounded && isGrounded && inJumpAnim)
        {
            inJumpAnim = false;
            anim.PlayLand();
        }

        // While dashing, ignore everything else
        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0f) isDashing = false;
            IsRunning = false;
            return;
        }

        // Left/right input and facing
        moveInput = Input.GetAxisRaw("Horizontal");
        
        if (moveInput != 0f) { // if -1 or 1 (left or right)
            facing = Mathf.Sign(moveInput);
        }

        // Run: hold Left Shift while moving
        if (!Input.GetKey(KeyCode.LeftShift)) runLocked = false;
        bool wantsToRun = Input.GetKey(KeyCode.LeftShift) && moveInput != 0f && !runLocked;
        if (wantsToRun && stamina.TryUse(runStaminaPerSecond * Time.deltaTime))
        {
            IsRunning = true;
            PlaySound(runSound); // run sound
        }
        else
        {
            if (wantsToRun) runLocked = true;
            IsRunning = false;
        }

        // Jump: coyote time + jump buffer make it feel forgiving
        coyoteTimer = isGrounded ? coyoteTime : coyoteTimer - Time.deltaTime;
        jumpBufferTimer = Input.GetKeyDown(KeyCode.Space) ? jumpBufferTime : jumpBufferTimer - Time.deltaTime;

        if (jumpBufferTimer > 0f && coyoteTimer > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpVelocity);
            jumpBufferTimer = 0f;
            coyoteTimer = 0f;
            inJumpAnim = true;
            anim.PlayJump();
        }

        // Releasing Space while still rising cuts the jump short (the Hollow Knight feel)
        if (Input.GetKeyUp(KeyCode.Space) && rb.linearVelocity.y > 0f)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);

        // Dash: Left Ctrl, in the direction you're facing, one per air time
        if (Input.GetKeyDown(KeyCode.LeftControl) && dashCooldownTimer <= 0f
            && (isGrounded || !airDashUsed) && stamina.TryUse(dashStaminaCost))
        {
            isDashing = true;
            dashTimer = dashDuration;
            dashCooldownTimer = dashCooldown;
            if (!isGrounded) airDashUsed = true;
            anim.PlayDash();
            PlaySound(dashSound);
        }
    }

    void FixedUpdate()
    {
        if (isDashing)
        {
            rb.gravityScale = 0f;                                   // Dash flies straight
            rb.linearVelocity = new Vector2(facing * dashSpeed, 0f);
            return;
        }

        // Heavier gravity while falling
        rb.gravityScale = rb.linearVelocity.y < 0f ? fallGravity : riseGravity;

        float speed = IsRunning ? runSpeed : moveSpeed;
        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y); // Keep vertical speed
    }

    // Checks for ground in a thin box just under the player's feet
    bool CheckGrounded()
    {
        Bounds b = col.bounds;
        Vector2 center = new Vector2(b.center.x, b.min.y - 0.05f);
        Vector2 size = new Vector2(b.size.x * 0.9f, 0.1f);
        return Physics2D.OverlapBox(center, size, 0f, groundLayer) != null;
    }

    void PlaySound(AudioClip clip)
    {
        if (clip == null) return;
        audioSource.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
        audioSource.PlayOneShot(clip); // OneShot lets sounds overlap instead of cutting each other off
    }
}