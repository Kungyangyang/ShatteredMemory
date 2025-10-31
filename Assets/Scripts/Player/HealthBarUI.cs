using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public Health playerHealth; 
    public Image fillImage;

    private float maxHealth;

    void Start()
    {
        if (playerHealth != null)
            maxHealth = playerHealth.maximumHealth;
    }

    void Update()
    {
        if (playerHealth != null && fillImage != null)
        {
            float healthPercent = (float)playerHealth.currentHealth / maxHealth;
            fillImage.fillAmount = healthPercent;
        }
    }
}
