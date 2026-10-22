using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public int maxHealth = 3;          // Set per enemy in the Inspector
    public Transform healthBarFill;    // The green bar object (set up in part D)

    protected int currentHealth;
    protected SpriteRenderer sr;
    protected Color originalColor;

    private Vector3 fillBaseScale;     // Full-health size of the bar
    private float fillBaseX;           // Full-health x position of the bar

    // "virtual" lets child scripts add to Start with "override"
    protected virtual void Start()
    {
        currentHealth = maxHealth;
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;

        if (healthBarFill != null)
        {
            fillBaseScale = healthBarFill.localScale;
            fillBaseX = healthBarFill.localPosition.x;
        }
        UpdateHealthBar();
    }

    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
            return;
        }

        // Flash white briefly
        sr.color = Color.white;
        Invoke("ResetColor", 0.1f);
    }

    void ResetColor()
    {
        sr.color = originalColor;
    }

    void UpdateHealthBar()
    {
        if (healthBarFill == null) return;

        float ratio = (float)currentHealth / maxHealth;

        // Shrink the bar, and shift it so the left edge stays fixed
        healthBarFill.localScale = new Vector3(fillBaseScale.x * ratio, fillBaseScale.y, fillBaseScale.z);
        healthBarFill.localPosition = new Vector3(
            fillBaseX - fillBaseScale.x * (1f - ratio) / 2f,
            healthBarFill.localPosition.y,
            healthBarFill.localPosition.z);
    }
}