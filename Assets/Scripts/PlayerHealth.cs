using UnityEngine.UI;
using UnityEngine;
using Image = UnityEngine.UI.Image;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Knockback")]
    public float knockbackForce = 8f;
    public float knockbackUp = 4f;
    public float knockbackTime = 0.2f;

    public GameObject deathScreen; // The "YOU DIED" panel
    public Image healthFront; // red bar (current health)
    public Image healthBack; // orange bar (recoverable health)
    public int maxHealth = 10;
    public SpriteRenderer sprite;          // The Visual child's Sprite Renderer
    public Color hitColor = Color.red;     // Colour the character flashes
    public float flashDuration = 0.1f;     // How long each flash lasts
    public int flashCount = 2;             // Number of blinks

    // hit enemy within x seconds to regain health
    public float regainWindow = 3f;
    private int currentHealth;
    private int recoverableHealth = 0;
    private float regainTimer = 0f; // count down for regainWindow

    void Start()
    {
        currentHealth = maxHealth;
        UpdateBars();
    }

    void Update()
    {
        if (recoverableHealth > 0)
        {
            regainTimer = regainTimer - Time.deltaTime;

            if (regainTimer <= 0f)
            {
                recoverableHealth = 0; // time is up, cant regain health
                Debug.Log("Regain window expired.");
                UpdateBars();
            }
        }
    }

    // Enemy.cs uses this to hurt player
    public void TakeDamage(int damage, Transform source = null)
    {
        // already dead: ignore further hits (health was going negative)
        if (!enabled) return;

        currentHealth = currentHealth - damage;
        recoverableHealth = recoverableHealth + damage;
        regainTimer = regainWindow; // 3f

        Debug.Log("Player health: " + currentHealth);
        Debug.Log("Recoverable health: " + recoverableHealth);

        UpdateBars();

        StartCoroutine(HitFlash()); // Red flash when hit

        // Push the player away from whatever hit them
        if (source != null)
        {
            float dir = Mathf.Sign(transform.position.x - source.position.x);
            if (dir == 0f) dir = -1f; // Exactly overlapping: pick right
            GetComponent<PlayerMovement>().Knockback(new Vector2(dir * knockbackForce, knockbackUp), knockbackTime);
        }

        // reload scene when player dies
        if (currentHealth <= 0)
        {
            // Guard so Die() only runs once
            if (!enabled) return;
            enabled = false;
            Die();
        }
    }

    // Called when the player hits an enemy. Regains at most 'amount' HP per hit
    public void RegainHealth(int amount)
    {
        if (recoverableHealth <= 0) return;

        // Can't regain more than what's recoverable (orange bar)
        int healed = Mathf.Min(amount, recoverableHealth);

        // Can't go above max health either
        healed = Mathf.Min(healed, maxHealth - currentHealth);

        currentHealth += healed;
        recoverableHealth -= healed; // The rest stays orange until the timer runs out

        Debug.Log("Regained " + healed + "! Player health: " + currentHealth);

        UpdateBars();
    }

    void UpdateBars()
    {
        if (healthFront != null)
            healthFront.fillAmount = (float)currentHealth / maxHealth;
        if (healthBack != null)
            healthBack.fillAmount = (float)(currentHealth + recoverableHealth) / maxHealth;
    }

    // Blinks the sprite between the hit colour and normal
    IEnumerator HitFlash()
    {
        if (sprite == null) yield break; // Nothing assigned, skip

        for (int i = 0; i < flashCount; i++)
        {
            sprite.color = hitColor;
            yield return new WaitForSeconds(flashDuration);
            sprite.color = Color.white; // White = the sprite's normal colours
            yield return new WaitForSeconds(flashDuration);
        }
    }
    // Shows the death screen, then reloads the scene after a delay
    void Die()
    {
        if (deathScreen != null) deathScreen.SetActive(true);

        // Play the death animation
        GetComponent<PlayerAnimation>().PlayDeath();

        // Stop the player from moving or attacking
        GetComponent<PlayerMovement>().enabled = false;
        GetComponent<PlayerAttack>().enabled = false;
        GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;

        // Reload after 2.5 seconds
        Invoke("Restart", 2.5f);
    }

    void Restart()
    {
        Time.timeScale = 1f; // in case a hit-stop was active when we died
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
