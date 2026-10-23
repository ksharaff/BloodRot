using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class PlayerStamina : MonoBehaviour
{
    public Image staminaFront;
    public Image staminaBack;
    public float maxStamina = 100f;
    public float regenRate = 30f;
    public float regenDelay = 0.8f;

    private float currentStamina;
    private float regenTimer = 0f;
    void Start()
    {
        currentStamina = maxStamina;
        UpdateBar();
    }

    void Update()
    {
        if(regenTimer>0)
        {
            regenTimer = regenTimer - Time.deltaTime;
        }
        else if(currentStamina < maxStamina)
        {
            currentStamina = Mathf.Min(currentStamina + regenRate * Time.deltaTime, maxStamina);
        }
        UpdateBar();
    }

//  checks if there is enough stamina to use in the first place
    public bool TryUse(float amount)
    {
        if(currentStamina<amount) return false;

        currentStamina = currentStamina - amount;
        regenTimer = regenDelay;
        return true;
    }

    void UpdateBar()
    {
        if(staminaFront != null)
        {
            staminaFront.fillAmount = currentStamina / maxStamina;
        }
    }
}
