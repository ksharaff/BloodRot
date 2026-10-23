using UnityEngine.UI;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class PlayerHealth : MonoBehaviour
{
    public GameObject deathScreen; // The "YOU DIED" panel
    public Image healthFront; // red bar (current health)
    public Image healthBack; // orange bar (recoverable health)
    public int maxHealth = 10;

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

            if(regainTimer <= 0f)
            {
                recoverableHealth=0; // time is up, cant regain health
                Debug.Log("Regain window expired.");
                UpdateBars();
            }
        }
    }
    
    // Enemy.cs uses this to hurt player
    public void TakeDamage(int damage)
    {
        // already dead: ignore further hits (health was going negative)
        if (!enabled) return;

        currentHealth = currentHealth - damage;
        recoverableHealth = recoverableHealth + damage;
        regainTimer = regainWindow; // 3f

        Debug.Log("Player health: " + currentHealth);
        Debug.Log("Recoverable health: " + recoverableHealth);

        UpdateBars();

        // reload scene when player dies
        if (currentHealth <= 0)
        {
            // Guard so Die() only runs once
            if (!enabled) return;
            enabled = false;
            Die();
        }
    }

    public void RegainHealth()
    {
        if(recoverableHealth<=0) return;

        // heal what is regainable but not above max health
        currentHealth = Mathf.Min(currentHealth+recoverableHealth, maxHealth);
        recoverableHealth=0;

        Debug.Log("Regained! Player health: " + currentHealth);
        
        UpdateBars();
    }

    void UpdateBars()
    {
        if(healthFront!=null)
            healthFront.fillAmount = (float)currentHealth / maxHealth;
        if(healthBack!=null)
            healthBack.fillAmount = (float)(currentHealth + recoverableHealth) / maxHealth;
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
