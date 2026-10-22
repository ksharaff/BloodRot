using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public int damage = 1;
    public float attackRange = 2f;      // How far the attack reaches
    public float attackCooldown = 0.4f;   // Time between attacks

    private float cooldownTimer = 0f;
    private Vector2 lastDirection = Vector2.right; // Direction we attack toward
    private PlayerStamina stamina;
    private HitStop hitStop;

    void Start()
    {
        stamina = GetComponent<PlayerStamina>();
        hitStop = GetComponent<HitStop>();
    }

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        // Track the direction the player is moving, to aim the attack
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (input != Vector2.zero) lastDirection = input.normalized;

        // Left mouse click or J attacks
        if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Q)) && cooldownTimer <= 0f && stamina.TryUse(20f))
        {
            cooldownTimer = attackCooldown;
            Attack();
        }
    }

    void Attack()
    {
        // Check a circle in front of the player for anything with a collider
        Vector2 attackPoint = (Vector2)transform.position + lastDirection * attackRange * 0.5f;
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint, attackRange * 0.5f);

        foreach (Collider2D hit in hits)
        {
            // Works for any enemy type, since all inherit EnemyBase
            EnemyBase enemy = hit.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                GetComponent<PlayerHealth>().RegainHealth();
                hitStop.Stop();
            }
        }
    }

// shows attack area as a red circle in scene view (debug only)
    void OnDrawGizmosSelected()
    {
        Vector2 point = (Vector2)transform.position + lastDirection * attackRange * 0.5f;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(point, attackRange * 0.5f);
    }
}