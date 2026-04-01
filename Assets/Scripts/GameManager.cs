using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Simple game manager that watches for player death
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
    }

    private void Update()
    {
        // Keep reference valid if the player was spawned later.
        if (playerHealth == null)
        {
            ResolvePlayerHealthReference();
            return;
        }

        // Detect player death once and schedule restart.
        if (!restartQueued && playerHealth.health <= 0f)
        {
            restartQueued = true;
            Debug.Log($"Player died. Restarting scene in {restartDelay} seconds...");
            Invoke(nameof(RestartCurrentScene), restartDelay);
        }
    }

    private void ResolvePlayerHealthReference()
    {
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            playerHealth = playerObject.GetComponent<PlayerHealth>();
        }
    }

    private void RestartCurrentScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        Debug.Log($"Reloading scene: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }
}
