using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float moveSpeed = 5f;
    public float dodgeSpeed = 14f;
    public float dodgeDuration = 0.2f;
    public float dodgeCooldown = 0.5f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 dodgeDirection;
    private Vector2 lastFacing = Vector2.right;

    private bool isDodging = false;
    private float dodgeTimer = 0f;
    private float cooldownTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if(cooldownTimer > 0f)
        {
            cooldownTimer = cooldownTimer - Time.deltaTime;
        }

        if(isDodging)
        {
            dodgeTimer = dodgeTimer - Time.deltaTime;
            
            if(dodgeTimer <= 0f)
            {
                isDodging = false;
                return;
            }
        }

        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput = moveInput.normalized;

        // remember last direction we moved
        if(moveInput!=Vector2.zero)
        {
            lastFacing = moveInput;
        }
        if(Input.GetKeyDown(KeyCode.LeftShift) && cooldownTimer <= 0f)
        {
            print("dodged!");
            isDodging=true;
            dodgeTimer = dodgeDuration;
            cooldownTimer = dodgeTimer;

            // dodge the direction u are moving
            dodgeDirection = moveInput!=Vector2.zero ? moveInput:lastFacing;
        
        }
    }

    void FixedUpdate() {
        if (isDodging) 
            rb.linearVelocity = dodgeDirection * dodgeSpeed;
        
        rb.linearVelocity = moveInput * moveSpeed;
    }
}
