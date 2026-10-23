using UnityEngine;

public class EnemyStriker : EnemyBase
{
    public float moveSpeed = 2.5f;
    public float attackRange = 1.5f;
    public float windupTime = 0.6f;
    public float strikeTime = 0.15f;
    public float recoverTime = 0.8f;
    public int damage = 2;
    public float detectRange = 6f;

    private enum State { Chasing, Windup, Strike, Recover }
    private State state = State.Chasing;
    private float stateTimer = 0f;
    private Rigidbody2D rb;
    private Transform player;

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        switch (state)
        {
            case State.Chasing:
                // Start winding up when close enough
                if (distance <= attackRange)
                {
                    state = State.Windup;
                    stateTimer = windupTime;
                    sr.color = Color.yellow; // The warning
                }
                break;

            case State.Windup:
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0f)
                {
                    state = State.Strike;
                    stateTimer = strikeTime;
                    sr.color = Color.red;

                    // Hit the player if they're still in range
                    if (distance <= attackRange)
                        player.GetComponent<PlayerHealth>().TakeDamage(damage, transform);
                }
                break;

            case State.Strike:
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0f)
                {
                    state = State.Recover;
                    stateTimer = recoverTime;
                    sr.color = originalColor;
                }
                break;

            case State.Recover:
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0f) state = State.Chasing;
                break;
        }
    }
    // After a hit flash, go back to the color that matches the current attack state,
    // so getting hit doesn't hide the yellow warning
    protected override Color GetRestColor()
    {
        if (state == State.Windup) return Color.yellow;
        if (state == State.Strike) return Color.red;
        return originalColor;
    }

    void FixedUpdate()
    {
        if (player == null) return;

        if (state == State.Chasing)
        {
            float dx = player.position.x - rb.position.x;

            // Player too far away: stand still
            if (Mathf.Abs(dx) > detectRange)
            {
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
                return;
            }

            float dirX = Mathf.Abs(dx) > 0.1f ? Mathf.Sign(dx) : 0f;
            rb.linearVelocity = new Vector2(dirX * moveSpeed, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y); // Stand still while attacking
        }
    }
    
}
