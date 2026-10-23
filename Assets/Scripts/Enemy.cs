using System;
using UnityEngine;

public class Enemy : EnemyBase   // Inherits health, flash and bar from EnemyBase
{
    public float moveSpeed = 2f;
    public int contactDamage = 1;
    public float attackCooldown = 1f;
    public float detectRange = 6f; // Enemy only chases when the player is this close

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
        float dx = player.position.x - rb.position.x;
        
        // player too far away: dont chase
        if(Math.Abs(dx) > detectRange)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }
        float dirX = Mathf.Abs(dx) > 0.1f ? Mathf.Sign(dx) : 0f; // Stop when lined up, no jitter
        rb.linearVelocity = new Vector2(dirX * moveSpeed, rb.linearVelocity.y); // Keep falling speed
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && attackTimer <= 0f)
        {
            attackTimer = attackCooldown;
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(contactDamage, transform);
        }
    }
}