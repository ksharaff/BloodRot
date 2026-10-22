using UnityEngine;

public class EnemyStriker : EnemyBase
{
    public float moveSpeed = 2.5f;
    public float attackRange = 1.5f;
    public float windupTime = 0.6f;
    public float strikeTime = 0.15f;
    public float recoverTime = 0.8f;
    public int damage = 2;

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
                        player.GetComponent<PlayerHealth>().TakeDamage(damage);
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
    void FixedUpdate()
    {
        if (player == null) return;

        // Only move while chasing; stand still during attack states
        if (state == State.Chasing)
        {
            Vector2 direction = ((Vector2)player.position - rb.position).normalized;
            rb.linearVelocity = direction * moveSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
    
}
