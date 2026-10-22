using UnityEngine;

public class Enemy : EnemyBase   // Inherits health, flash and bar from EnemyBase
{
    public float moveSpeed = 2f;
    public int contactDamage = 1;
    public float attackCooldown = 1f;

    private Rigidbody2D rb;
    private Transform player;
    private float attackTimer = 0f;

    protected override void Start()
    {
        base.Start(); // Runs the EnemyBase setup first
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (attackTimer > 0f) attackTimer -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (player == null) return;
        Vector2 direction = ((Vector2)player.position - rb.position).normalized;
        rb.linearVelocity = direction * moveSpeed;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && attackTimer <= 0f)
        {
            attackTimer = attackCooldown;
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(contactDamage);
        }
    }
}