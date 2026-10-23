using UnityEngine;
[RequireComponent(typeof(AudioSource))] // Unity auto-adds an Audio Source and won't let it be removed
public class PlayerAttack : MonoBehaviour
{
    [Header("Sounds")]
    public AudioClip lightSwingSound;
    public AudioClip heavySwingSound;
    public AudioClip hitSound;
    [Range(0f, 0.3f)] public float pitchVariation = 0.1f;

    [Header("Light attack (left click / Q)")]
    public int damage = 1;
    public float attackRange = 2f;
    public float attackCooldown = 0.4f;
    public float staminaCost = 20f;

    [Header("Heavy attack (right click / E)")]
    public int heavyDamage = 3;
    public float heavyRange = 2.5f;        // Slightly longer reach
    public float heavyCooldown = 0.9f;     // Slower, so it can't be spammed
    public float heavyStaminaCost = 40f;

    private float cooldownTimer = 0f;      // Shared: you can't light-attack mid heavy swing
    private Vector2 lastDirection = Vector2.right;
    private PlayerStamina stamina;
    private HitStop hitStop;
    private PlayerAnimation anim;
    private PlayerHealth health;
    private AudioSource audioSource;
    private float facingX = 1f;

    void Start()
    {
        stamina = GetComponent<PlayerStamina>();
        hitStop = GetComponent<HitStop>();
        anim = GetComponent<PlayerAnimation>();
        health = GetComponent<PlayerHealth>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        // Holding W/S aims the attack up/down, otherwise it goes the way you're facing
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        if (h != 0f) facingX = Mathf.Sign(h);
        lastDirection = v != 0f ? new Vector2(0f, Mathf.Sign(v)) : new Vector2(facingX, 0f);

        if (cooldownTimer > 0f) return; // Still recovering from the last swing

        // Light attack
        if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Q)) && stamina.TryUse(staminaCost))
        {
            cooldownTimer = attackCooldown;
            anim.PlayAttack();
            PlaySound(lightSwingSound);
            DealDamage(damage, attackRange);
        }
        // Heavy attack: right mouse button is 1
        else if ((Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.E)) && stamina.TryUse(heavyStaminaCost))
        {
            cooldownTimer = heavyCooldown;
            anim.PlayHeavyAttack();
            PlaySound(heavySwingSound);
            DealDamage(heavyDamage, heavyRange);
        }
    }

    // Shared by both attacks: hits every enemy in a circle in front of the player
    void DealDamage(int amount, float range)
    {
        Vector2 attackPoint = (Vector2)transform.position + lastDirection * range * 0.5f;
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint, range * 0.5f);

        bool hitSomething = false;

        foreach (Collider2D hit in hits)
        {
            EnemyBase enemy = hit.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                enemy.TakeDamage(amount);
                health.RegainHealth();
                hitStop.Stop();
                hitSomething = true;
            }
        }

        // One hit sound per attack, even if it hits several enemies
        if (hitSomething) PlaySound(hitSound);
    }

    void PlaySound(AudioClip clip)
    {
        if (clip == null) return;
        audioSource.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
        audioSource.PlayOneShot(clip); // OneShot lets sounds overlap instead of cutting each other off
    }

    // Draws both attack areas in the Scene view when Player is selected
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere((Vector2)transform.position + lastDirection * attackRange * 0.5f, attackRange * 0.5f);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere((Vector2)transform.position + lastDirection * heavyRange * 0.5f, heavyRange * 0.5f);
    }
}