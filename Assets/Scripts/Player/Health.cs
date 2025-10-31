using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    [Header("Team Settings")]
    public int teamId = 0;

    [Header("Health Settings")]
    public int defaultHealth = 100;
    public int maximumHealth = 100;
    public int currentHealth = 100;
    public float invincibilityTime = 2f;
    public float flickerSpeed = 0.1f;

    private bool isInvincible = false;
    private float invincibleTimer = 0f;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;

        currentHealth = Mathf.Clamp(defaultHealth, 0, maximumHealth);
    }

    void Update()
    {
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer <= 0)
            {
                isInvincible = false;
                if (spriteRenderer != null)
                    spriteRenderer.color = originalColor;
            }
        }
    }

    public void TakeDamage(int amount)
    {
        if (isInvincible) return;

        currentHealth -= amount;

        if (spriteRenderer != null)
            spriteRenderer.color = Color.red;

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(InvincibilityCoroutine());
        }
    }

    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        invincibleTimer = invincibilityTime;

        float elapsed = 0f;

        while (elapsed < invincibilityTime)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.3f);
                yield return new WaitForSeconds(flickerSpeed);
                spriteRenderer.color = originalColor;
                yield return new WaitForSeconds(flickerSpeed);
            }

            elapsed += flickerSpeed * 2f;
        }

        isInvincible = false;

        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;
    }

    private void Die()
    {
        SceneManager.LoadScene("Menu");
    }
}
