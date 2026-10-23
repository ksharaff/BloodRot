using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public Animator animator;               // The Animator on Visual
    public SpriteRenderer spriteRenderer;   // The Sprite Renderer on Visual
    public bool spriteFacesRight = true;    // Tick/untick depending on your art

    private Rigidbody2D rb;
    private PlayerMovement movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        movement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        // Walk animation plays whenever the player is moving
        bool moving = Mathf.Abs(rb.linearVelocity.x) > 0.1f;
        animator.SetBool("isWalking", moving);
        animator.SetBool("isRunning", movement.IsRunning);

        // Flip the sprite depending on left/right movement
        float x = rb.linearVelocity.x;
        if (x > 0.1f) spriteRenderer.flipX = !spriteFacesRight;
        else if (x < -0.1f) spriteRenderer.flipX = spriteFacesRight;
    }

    // Called by PlayerAttack when an attack starts
    public void PlayAttack()
    {
        animator.SetTrigger("Attack1");
    }

    // Called by PlayerAttack when a heavy attack starts
    public void PlayHeavyAttack()
    {
        animator.SetTrigger("Attack2");
    }

    // Called by PlayerHealth when the player dies
    public void PlayDeath()
    {
        animator.SetBool("isWalking", false); // so Walk returns to Idle, then Idle goes to Death
        animator.SetBool("Death", true);
        enabled = false; // stop updating walk/flip after death
    }

    public void PlayDash()
    {
        animator.SetTrigger("Dash");
    }

    public void PlayJump()
    {
        animator.SetBool("Jump-Land", false);
        animator.SetTrigger("Jump");
    }

    public void PlayLand()
    {
        animator.SetBool("Jump-Land", true);
    }
}