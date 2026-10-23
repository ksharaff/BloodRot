using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    public Transform visual;                     // The Visual child (the sprite we lift up)
    public KeyCode jumpKey = KeyCode.Backspace;
    public float staminaCost = 15f;

    [Header("Tap (small hop) -> full charge (big leap)")]
    public float maxChargeTime = 0.6f;           // Seconds of holding for a full charge
    public float minHeight = 0.5f;
    public float maxHeight = 2f;
    public float minDuration = 0.35f;            // Airtime for a tap
    public float maxDuration = 0.8f;             // Airtime for a full charge
    public float minDistance = 1f;               // Forward travel for a tap
    public float maxDistance = 5f;               // Forward travel for a full charge

    public bool IsJumping { get; private set; }
    // PlayerMovement uses this to carry the player forward during the leap
    public Vector2 JumpVelocity { get; private set; }

    private bool isCharging = false;
    private float chargeTimer = 0f;
    private float jumpTimer = 0f;
    private float currentHeight, currentDuration;
    private Vector3 visualStartPos;
    private Vector2 lastDirection = Vector2.right;
    private PlayerStamina stamina;
    private PlayerAnimation anim;

    void Start()
    {
        stamina = GetComponent<PlayerStamina>();
        anim = GetComponent<PlayerAnimation>();
        visualStartPos = visual.localPosition;
    }

    void Update()
    {
        // Remember the last direction we moved, so we can leap that way
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (input != Vector2.zero) lastDirection = input.normalized;

        if (!IsJumping)
        {
            // Start charging when the key is pressed
            if (Input.GetKeyDown(jumpKey)) { isCharging = true; chargeTimer = 0f; }

            if (isCharging)
            {
                chargeTimer += Time.deltaTime;

                // Jump when the key is released, or automatically at full charge
                if (Input.GetKeyUp(jumpKey) || chargeTimer >= maxChargeTime)
                {
                    isCharging = false;
                    if (stamina.TryUse(staminaCost))
                        StartJump(Mathf.Clamp01(chargeTimer / maxChargeTime));
                }
            }
            return;
        }

        // In the air
        jumpTimer += Time.deltaTime;
        float t = jumpTimer / currentDuration; // 0 at takeoff, 1 at landing

        if (t >= 1f)
        {
            IsJumping = false;
            JumpVelocity = Vector2.zero;
            visual.localPosition = visualStartPos;
            anim.PlayLand();
            return;
        }

        // Arc: 0 at the start, peak in the middle, 0 at the end
        float height = 4f * currentHeight * t * (1f - t);
        visual.localPosition = visualStartPos + Vector3.up * height;
    }

    // charge is 0 (tap) to 1 (fully charged)
    void StartJump(float charge)
    {
        currentHeight = Mathf.Lerp(minHeight, maxHeight, charge);
        currentDuration = Mathf.Lerp(minDuration, maxDuration, charge);
        float distance = Mathf.Lerp(minDistance, maxDistance, charge);

        // Speed needed to cover that distance during the airtime
        JumpVelocity = lastDirection * (distance / currentDuration);

        IsJumping = true;
        jumpTimer = 0f;
        anim.PlayJump();
    }
}