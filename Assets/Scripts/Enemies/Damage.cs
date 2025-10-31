using UnityEngine;

public class Damage : MonoBehaviour
{
    [Header("Team Settings")]
    public int teamId = -1;

    [Header("Damage Settings")]
    public int damageAmount = 1;
    public bool destroyAfterDamage = false;
    public bool dealDamageOnTriggerEnter = true;
    public bool dealDamageOnTriggerStay = true;
    public bool dealDamageOnCollision = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (dealDamageOnTriggerEnter)
            DealDamage(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (dealDamageOnTriggerStay)
            DealDamage(other);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (dealDamageOnCollision)
            DealDamage(collision.collider);
    }

    private void DealDamage(Collider2D target)
    {
        Health health = target.GetComponent<Health>();
        if (health != null && health.teamId != teamId)
        {
            health.TakeDamage(damageAmount);
            if (destroyAfterDamage)
                Destroy(gameObject);
        }
    }
}
