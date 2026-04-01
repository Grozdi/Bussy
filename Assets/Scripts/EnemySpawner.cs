using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple enemy spawner.
/// Spawns enemy prefabs at random positions inside a radius while
/// keeping the number of alive enemies under a max limit.
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [Tooltip("Enemy prefab to spawn.")]
    public GameObject enemyPrefab;

    [Tooltip("Seconds between spawn attempts.")]
    public float spawnRate = 2f;

    [Tooltip("Maximum number of alive enemies at once.")]
    public int maxEnemies = 10;

    [Tooltip("Spawn radius around this spawner.")]
    public float spawnRadius = 12f;

    // Tracks currently alive spawned enemies.
    private readonly List<GameObject> currentEnemies = new List<GameObject>();
    private float nextSpawnTime;

    private void Update()
    {
        // Keep the list clean by removing destroyed enemies.
        CleanupDestroyedEnemies();

        // Only attempt spawn when under max and when spawn timer is ready.
        if (currentEnemies.Count >= maxEnemies)
        {
            return;
        }

        if (Time.time < nextSpawnTime)
        {
            return;
        }

        TrySpawnEnemy();
    }

    private void TrySpawnEnemy()
    {
        if (enemyPrefab == null)
        {
            Debug.LogWarning("EnemySpawner: enemyPrefab is not assigned.");
            nextSpawnTime = Time.time + spawnRate;
            return;
        }

        Vector3 spawnPosition = GetRandomSpawnPosition();
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

        if (newEnemy != null)
        {
            currentEnemies.Add(newEnemy);
        }

        nextSpawnTime = Time.time + spawnRate;
    }

    private Vector3 GetRandomSpawnPosition()
    {
        // Pick a random point on XZ plane within radius.
        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        return new Vector3(
            transform.position.x + randomCircle.x,
            transform.position.y,
            transform.position.z + randomCircle.y);
    }

    private void CleanupDestroyedEnemies()
    {
        for (int i = currentEnemies.Count - 1; i >= 0; i--)
        {
            if (currentEnemies[i] == null)
            {
                currentEnemies.RemoveAt(i);
            }
        }
    }
}
