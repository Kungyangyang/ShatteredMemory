using UnityEngine;

public class PoolableObject : MonoBehaviour
{
    public string poolTag;
    public float lifetime = 3f;

    private void OnEnable()
    {
        // Automatically return to pool after lifetime expires
        if (lifetime > 0)
        {
            Invoke(nameof(ReturnToPool), lifetime);
        }
    }

    private void OnDisable()
    {
        // Cancel any pending return calls
        CancelInvoke();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Return to pool on collision (optional)
        ReturnToPool();
    }

    public void ReturnToPool()
    {
        if (ObjectPool.Instance != null)
        {
            ObjectPool.Instance.ReturnToPool(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}