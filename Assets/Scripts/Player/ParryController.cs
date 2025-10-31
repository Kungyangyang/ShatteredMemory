using UnityEngine;
using System.Collections;

public class ParryController : MonoBehaviour
{
    [Header("Parry Settings")]
    public float parryRange = 2f;
    public float parryCooldown = 1f;
    public float freezeDuration = 1f;
    public LayerMask enemyLayer = 1;

    [Header("Heal Settings")]
    public int minHealAmount = 15;
    public int maxHealAmount = 30;

    [Header("Visual Effects")]
    public GameObject parryEffectPrefab;
    public AudioClip parrySound;

    private bool canParry = true;
    private Camera mainCamera;
    private MusicTimer musicTimer;
    private AudioSource audioSource;
    private Health playerHealth;

    void Start()
    {
        mainCamera = Camera.main;
        musicTimer = UnityEngine.Object.FindFirstObjectByType<MusicTimer>();
        audioSource = GetComponent<AudioSource>();
        playerHealth = GetComponent<Health>();
        
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && canParry)
        {
            AttemptParry();
        }
    }

    void AttemptParry()
    {
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, parryRange, enemyLayer);
        
        foreach (Collider2D enemy in enemiesInRange)
        {
            if (enemy.CompareTag("Ravager") || enemy.GetComponent<RavagerDamage>() != null)
            {
                SuccessfulParry(enemy.transform);
                return;
            }
        }
    }

    void SuccessfulParry(Transform enemy)
    {
        StartCoroutine(ParrySequence(enemy));
    }

    private IEnumerator ParrySequence(Transform enemy)
    {
        canParry = false;
        
        HealPlayer();
        
        if (parrySound != null)
        {
            audioSource.PlayOneShot(parrySound);
        }

        if (parryEffectPrefab != null)
        {
            Instantiate(parryEffectPrefab, enemy.position, Quaternion.identity);
        }

        StartCoroutine(FreezeFrameEffect());

        Destroy(enemy.gameObject);

        yield return new WaitForSeconds(parryCooldown);
        canParry = true;
    }

    private void HealPlayer()
    {
        if (playerHealth != null)
        {
            int healAmount = Random.Range(minHealAmount, maxHealAmount + 1);
            playerHealth.currentHealth = Mathf.Min(playerHealth.currentHealth + healAmount, playerHealth.maximumHealth);
        }
    }

    private IEnumerator FreezeFrameEffect()
    {
        Rigidbody2D[] allRigidbodies = UnityEngine.Object.FindObjectsByType<Rigidbody2D>(FindObjectsSortMode.None);
        Vector2[] storedVelocities = new Vector2[allRigidbodies.Length];
        float[] storedAngularVelocities = new float[allRigidbodies.Length];
        
        for (int i = 0; i < allRigidbodies.Length; i++)
        {
            storedVelocities[i] = allRigidbodies[i].linearVelocity;
            storedAngularVelocities[i] = allRigidbodies[i].angularVelocity;
            allRigidbodies[i].simulated = false;
        }

        if (musicTimer != null)
        {
            musicTimer.PauseMusicWithFreeze();
        }

        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(freezeDuration);

        Time.timeScale = 1f;

        for (int i = 0; i < allRigidbodies.Length; i++)
        {
            if (allRigidbodies[i] != null)
            {
                allRigidbodies[i].simulated = true;
                allRigidbodies[i].linearVelocity = storedVelocities[i];
                allRigidbodies[i].angularVelocity = storedAngularVelocities[i];
            }
        }

        if (musicTimer != null)
        {
            musicTimer.ResumeMusicAfterFreeze();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, parryRange);
    }
}