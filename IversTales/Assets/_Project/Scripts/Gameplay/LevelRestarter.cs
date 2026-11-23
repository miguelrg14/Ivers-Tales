using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Reloads the current level. Used as a kill zone (falling off the map) and by
/// the player when it dies.
/// </summary>
public class LevelRestarter : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // Enemy mortar shells also fall into the kill zones; only the player restarts the level.
        if (other.GetComponentInParent<PlayerController>() != null)
            RestartGame();
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
