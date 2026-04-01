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

    [Tooltip("Optional player reference used to avoid spawning too close.")]
    public Transform player;

    [Tooltip("Seconds between spawn attempts.")]
    public float spawnRate = 2f;

    [Tooltip("Maximum number of alive enemies at once.")]
    public int maxEnemies = 10;

    [Tooltip("Spawn radius around this spawner.")]
    public float spawnRadius = 12f;

    [Tooltip("Minimum allowed distance from player when spawning.")]
    public float minDistanceFromPlayer = 6f;

    [Tooltip("How many random position attempts to find a valid spawn point.")]
    public int maxSpawnAttempts = 8;

    // Tracks currently alive spawned enemies.
    private readonly List<GameObject> aliveEnemies = new List<GameObject>();
    private float nextSpawnTime;

    private void Awake()
    {
        // Optional fallback to player tagged object.
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }
    }

    private void Update()
    {
        // Keep the list clean by removing destroyed enemies.
        CleanupDestroyedEnemies();

        // Only attempt spawn when under max and when spawn timer is ready.
        if (aliveEnemies.Count >= maxEnemies)
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

        Vector3 spawnPosition;
        if (!TryGetValidSpawnPosition(out spawnPosition))
        {
            // Skip this spawn cycle if no valid position was found.
            nextSpawnTime = Time.time + spawnRate;
            return;
        }

        GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        if (newEnemy != null)
        {
            aliveEnemies.Add(newEnemy);
        }

        nextSpawnTime = Time.time + spawnRate;
    }

    private bool TryGetValidSpawnPosition(out Vector3 spawnPosition)
    {
        for (int i = 0; i < Mathf.Max(1, maxSpawnAttempts); i++)
        {
            Vector3 candidate = GetRandomSpawnPosition();

            // If no player reference exists, accept the position.
            if (player == null)
            {
                spawnPosition = candidate;
                return true;
            }

            float distanceToPlayer = Vector3.Distance(candidate, player.position);
            if (distanceToPlayer >= minDistanceFromPlayer)
            {
                spawnPosition = candidate;
                return true;
            }
        }

        spawnPosition = transform.position;
        return false;
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
        for (int i = aliveEnemies.Count - 1; i >= 0; i--)
        {
            if (aliveEnemies[i] == null)
            {
                aliveEnemies.RemoveAt(i);
            }
        }
    }
}
