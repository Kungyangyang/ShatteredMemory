using UnityEngine;

public class ShooterEnemy : MonoBehaviour
{
    public GameObject EnemyBullet;
    public Transform firePoint;
    public float shootInterval = 2f;
    public float detectionRange = 8f;

    private Transform player;
    private float shootTimer;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null) return;

        // Check if player is in range
        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= detectionRange)
        {
            // Aim at player
            Vector2 aimDir = (player.position - firePoint.position).normalized;
            float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
            firePoint.rotation = Quaternion.Euler(0, 0, angle);

            // Shoot on timer
            shootTimer -= Time.deltaTime;
            if (shootTimer > 2)
            {
                shootTimer = 0;
                Shoot(aimDir);
            }
        }
    }

    void Shoot(Vector2 direction)
    {
        Instantiate(EnemyBullet, firePoint.position, Quaternion.identity);
        GameObject bullet = Instantiate(EnemyBullet, firePoint.position, Quaternion.identity);
        EnemyBullet bulletScript = bullet.GetComponent<EnemyBullet>();
        if (bulletScript != null)
        {
            bulletScript.SetDirection(direction);
        }
    }
}
