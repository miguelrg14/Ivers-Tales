using System.Collections;
using IversTales.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Trigger at the end of a level. When the player walks in, waits a moment and
/// loads the next scene (Level1 → Level2 → Credits, see <see cref="GameScenes"/>).
/// </summary>
public class LevelExit : MonoBehaviour
{
    [Tooltip("Leave empty to follow the default level order.")]
    [SerializeField] string nextScene = "";
    [SerializeField] float delay = 1f;

    bool triggered;

    void OnTriggerEnter(Collider other)
    {
        if (triggered || !other.CompareTag("Player")) return;
        triggered = true;
        StartCoroutine(LoadNextScene());
    }

    IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(delay);
        string scene = string.IsNullOrEmpty(nextScene)
            ? GameScenes.NextAfter(SceneManager.GetActiveScene().name)
            : nextScene;
        SceneManager.LoadScene(scene);
    }
}
