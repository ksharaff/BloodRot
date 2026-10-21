using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine.UI;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class PlayerHealth : MonoBehaviour
{
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
        currentHealth = currentHealth - damage;
        recoverableHealth = recoverableHealth + damage;
        regainTimer = regainWindow; // 3f

        Debug.Log("Player health: " + currentHealth);
        Debug.Log("Recoverable health: " + recoverableHealth);

        UpdateBars();

        // reload scene when player dies
        if (currentHealth <= 0)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
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
}
