using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 3;
    public float moveSpeed = 2f;
    public int contactDamage = 1;
    public float attackCooldown = 1f;   // Time between hits on the player

    private int currentHealth;
    private Rigidbody2D rb;
    private Transform player;
    private float attackTimer = 0f;
    private SpriteRenderer sr; // used to change enemy color
    private Color originalColor; // remember normal color

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();

        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;

        Debug.Log("Enemy health: " + currentHealth);


        // Find the player by its tag
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (attackTimer > 0f) attackTimer -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (player == null) return;

        // Move straight toward the player
        Vector2 direction = ((Vector2)player.position - rb.position).normalized;
        rb.linearVelocity = direction * moveSpeed;
    }

    // Runs every physics step while touching another collider
    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && attackTimer <= 0f)
        {
            attackTimer = attackCooldown;
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(contactDamage);
        }
    }

    public void TakeDamage(int damage)
{
    currentHealth = currentHealth - damage;
    Debug.Log("Enemy health: " + currentHealth);

    if (currentHealth <= 0)
    {
        Destroy(gameObject);
        return;
    }

    // Flash white, then return to normal color
    sr.color = Color.white;
    Invoke("ResetColor", 0.1f);
}

void ResetColor()
    {
        sr.color = originalColor;
    }
}