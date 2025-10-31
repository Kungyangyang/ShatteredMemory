using UnityEngine;

public class RavagerDamage : MonoBehaviour
{
    [Header("Team Settings")]
    public int teamId = -1;

    [Header("Damage Settings")]
    public int damageAmount = 40;
    public bool destroyAfterDamage = false;
    public bool dealDamageOnTriggerEnter = true;
    public bool dealDamageOnTriggerStay = false;
    public bool dealDamageOnCollision = false;

    public bool canDamage = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (dealDamageOnTriggerEnter && canDamage)
            DealDamage(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (dealDamageOnTriggerStay && canDamage)
            DealDamage(other);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (dealDamageOnCollision && canDamage)
            DealDamage(collision.collider);
    }

    private void DealDamage(Collider2D target)
    {
        if (!canDamage) return;

        Health health = target.GetComponent<Health>();
        if (health != null && health.teamId != teamId)
        {
            health.TakeDamage(damageAmount);
            if (destroyAfterDamage)
                Destroy(gameObject);
        }
    }

    public void DisableDamage()
    {
        canDamage = false;
    }
}