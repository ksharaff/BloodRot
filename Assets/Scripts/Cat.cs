using UnityEngine;
[RequireComponent(typeof(AudioSource))] // Unity auto-adds an Audio Source and won't let it be removed

public class Cat : MonoBehaviour
{
    [Header("Sounds")]
    public AudioClip meowSound;
    public AudioClip attackSound;
    public float minMeowDelay = 5f;
    public float maxMeowDelay = 12f;
    
    public float speed = 4f;
    public float followDistance = 2f;     // Stays this far from the player
    public float teleportDistance = 12f;  // Snaps to the player if left too far behind
    public float detectRange = 6f;        // Sees enemies within this distance
    public float attackRange = 0.8f;      // Bites when this close
    public int damage = 1;
    public float attackCooldown = 1f;

    private Transform player;
    private Rigidbody2D rb; // catRb
    private Rigidbody2D playerRb;
    private SpriteRenderer sr;
    private Animator anim;
    private float attackTimer = 0f;
    private AudioSource audioSource;
    private float meowTimer;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        playerRb = player.GetComponent<Rigidbody2D>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        // The cat walks through the player and enemies, it only collides with the ground
        Collider2D myCol = GetComponent<Collider2D>();
        Physics2D.IgnoreCollision(myCol, player.GetComponent<Collider2D>());
        foreach (EnemyBase e in FindObjectsByType<EnemyBase>())
            Physics2D.IgnoreCollision(myCol, e.GetComponent<Collider2D>());

        audioSource = GetComponent<AudioSource>();
        meowTimer = Random.Range(minMeowDelay, maxMeowDelay);
    }

    void Update()
    {
        HandleMeow();
    }

    void FixedUpdate()
    {
        if (attackTimer > 0f) attackTimer -= Time.fixedDeltaTime;

        // Too far behind: teleport next to the player
        if (Mathf.Abs(player.position.x - rb.position.x) > teleportDistance)
        {
            rb.position = player.position;
            return;
        }

        float currentSpeed = speed;

        EnemyBase target = FindNearestEnemy();
        float dir = 0f;

        if (target != null)
        {
            float dx = target.transform.position.x - rb.position.x;
            if (Mathf.Abs(dx) > attackRange)
            {
                dir = Mathf.Sign(dx); // Run at the enemy
            }
            else if (attackTimer <= 0f)
            {
                // Close enough: bite
                PlaySound(attackSound);
                target.TakeDamage(damage);
                attackTimer = attackCooldown;
                anim.SetTrigger("Attack");
                sr.flipX = dx < 0f;
            }
        }
        else
        {
            // No enemy: follow the player
            float dx = player.position.x - rb.position.x;
            if (Mathf.Abs(dx) > followDistance)
            {
                dir = Mathf.Sign(dx);

                float playerSpeed = Mathf.Abs(playerRb.linearVelocity.x);
                float catchUp = (Mathf.Abs(dx) - followDistance) * 0.5f;
                currentSpeed = Mathf.Max(speed, playerSpeed) + catchUp;
            }
        }

        rb.linearVelocity = new Vector2(dir * currentSpeed, rb.linearVelocity.y); // Keep gravity's vertical speed
        if (dir != 0f) sr.flipX = dir < 0f; // Sprite faces right by default
        anim.SetBool("isWalking", dir != 0f);
    }

    // Returns the closest enemy inside detectRange, or null
    EnemyBase FindNearestEnemy()
    {
        EnemyBase nearest = null;
        float best = detectRange;
        foreach (EnemyBase e in FindObjectsByType<EnemyBase>())
        {
            float d = Vector2.Distance(transform.position, e.transform.position);
            if (d < best) { best = d; nearest = e; }
        }
        return nearest;
    }

    void HandleMeow()
    {
        meowTimer -= Time.deltaTime;
        if (meowTimer > 0f) return;

        if (meowSound != null)
        {
            audioSource.pitch = 1f + Random.Range(-0.1f, 0.1f); // Slight variation so it isn't identical every time
            audioSource.PlayOneShot(meowSound);
        }
        meowTimer = Random.Range(minMeowDelay, maxMeowDelay); // Pick the next wait
    }

    void PlaySound(AudioClip clip)
    {
        if(clip == null) return;
        audioSource.PlayOneShot(clip);
    }
}