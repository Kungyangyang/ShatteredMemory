using UnityEngine;

public class DamageHitbox : MonoBehaviour
{
    private Health parentHealth;

    void Start()
    {
        parentHealth = GetComponentInParent<Health>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        RavagerDamage ravagerDamage = other.GetComponent<RavagerDamage>();
        if (ravagerDamage != null && parentHealth != null)
        {
            parentHealth.TakeDamage(ravagerDamage.damageAmount);
        }
    }
}