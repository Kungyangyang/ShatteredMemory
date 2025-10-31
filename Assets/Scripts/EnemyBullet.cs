using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 7f;
    public float lifetime = 3f;
    private Vector2 direction;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.CompareTag("Player"))
        {
            Debug.Log("Player hit by enemy bullet!");
            Destroy(gameObject);
        }
        else if (hitInfo.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
