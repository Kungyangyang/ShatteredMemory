using UnityEngine;
using UnityEngine.UI;

public class DashBarUI : MonoBehaviour
{
    public PlayerMovement player;
    public Image fillImage;

    private float maxCooldown;

    void Start()
    {
        if (player != null)
            maxCooldown = player.dashCooldown;
    }

    void Update()
    {
        if (player != null && fillImage != null)
        {
            // If dash is cooling down, fill decreases
            float fill = 1f - Mathf.Clamp01(playerDashTimer());
            fillImage.fillAmount = fill;
        }
    }

    float playerDashTimer()
    {
        // Get how much time is left in cooldown
        float remaining = Mathf.Max(player.dashCooldownTimer, 0);
        return remaining / player.dashCooldown;
    }
}
