using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Simple game manager that listens for player death
/// and reloads the current scene after a short delay.
/// </summary>
public class GameManager : MonoBehaviour
{
    [Tooltip("Optional direct reference to PlayerHealth. If empty, found from Player tag.")]
    [SerializeField] private PlayerHealth playerHealth;

    [Tooltip("Delay before restarting the scene after player death.")]
    [SerializeField] private float restartDelay = 2.5f;

    private bool restartQueued;

    private void Awake()
    {
        ResolvePlayerHealthReference();
        SubscribeToPlayerDeath();
    }

    private void OnEnable()
    {
        // Re-subscribe safely if this object is toggled.
        SubscribeToPlayerDeath();
    }

    private void OnDisable()
    {
        UnsubscribeFromPlayerDeath();
    }

    private void Update()
    {
        // Keep reference valid if player is spawned later.
        if (playerHealth == null)
        {
            ResolvePlayerHealthReference();
            SubscribeToPlayerDeath();
        }
    }

    private void ResolvePlayerHealthReference()
    {
        if (playerHealth != null)
        {
            return;
        }

        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            playerHealth = playerObject.GetComponent<PlayerHealth>();
        }
    }

    private void SubscribeToPlayerDeath()
    {
        if (playerHealth == null)
        {
            return;
        }

        // Ensure no duplicate event subscription.
        playerHealth.OnPlayerDied -= HandlePlayerDied;
        playerHealth.OnPlayerDied += HandlePlayerDied;
    }

    private void UnsubscribeFromPlayerDeath()
    {
        if (playerHealth == null)
        {
            return;
        }

        playerHealth.OnPlayerDied -= HandlePlayerDied;
    }

    private void HandlePlayerDied()
    {
        // Prevent duplicate restart requests.
        if (restartQueued)
        {
            return;
        }

        restartQueued = true;
        Debug.Log($"Player death received by GameManager. Restarting scene in {restartDelay} seconds...");
        Invoke(nameof(RestartCurrentScene), restartDelay);
    }

    private void RestartCurrentScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        Debug.Log($"Reloading scene: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }
}
