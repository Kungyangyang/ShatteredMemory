using System;
using UnityEditor;
using UnityEngine;
using System.Collections;

public class Ravager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject telegraphPrefab;
    public Transform player;
    public float minSpawnInterval = 15f;
    public float maxSpawnInterval = 49f;
    public float sweepSpeed = 5f;
    public float enemyLifetime = 50f; 
    public float spawnOffsetX = 20f;
    public AudioClip telegraphSound;

    private void Start()
    {
        StartCoroutine(SpawnEnemyRoutine());
    }

    private IEnumerator SpawnEnemyRoutine()
    {
        while (true)
        {
            // Wait for a random interval between minSpawnInterval and maxSpawnInterval
            float spawnDelay = UnityEngine.Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(spawnDelay);

            // Spawn the telegraph at the player's current position (both x and y)
            Vector3 telegraphPosition = new Vector3(player.position.x, player.position.y, 0f);
            GameObject telegraph = Instantiate(telegraphPrefab, telegraphPosition, Quaternion.identity);

            if (telegraphSound != null)
            {
                AudioSource.PlayClipAtPoint(telegraphSound, telegraphPosition);
            }

            yield return new WaitForSeconds(2f);

            // Spawn the enemy to the left of the telegraph position
            Vector3 enemyPosition = new Vector3(telegraphPosition.x - spawnOffsetX, telegraphPosition.y, 0f);
            GameObject enemy = Instantiate(enemyPrefab, enemyPosition, Quaternion.identity);

            Destroy(telegraph);

            // Set the enemy's velocity to sweep from left to right
            Rigidbody2D enemyRigidbody = enemy.GetComponent<Rigidbody2D>();
            if (enemyRigidbody != null)
            {
                enemyRigidbody.linearVelocity = new Vector2(sweepSpeed, 0f);
            }

            Destroy(enemy, enemyLifetime);
        }
    }
}