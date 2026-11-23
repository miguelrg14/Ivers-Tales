using IversTales.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

/// <summary>Main menu buttons: play, options, credits and quit.</summary>
public class MainMenuManager : MonoBehaviour
{
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject optionsMenu;
    [SerializeField] GameObject creditsMenu;
    [SerializeField] GameObject loadingScenePanel;
    [Tooltip("Scene loaded by the Play button.")]
    [SerializeField, FormerlySerializedAs("SceneName")] string firstLevelScene = GameScenes.Level1;

    void Start()
    {
        Time.timeScale = 1f;
        ActivateMainMenu(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /// <summary>true shows the main menu, false shows the options menu.</summary>
    public void ActivateMainMenu(bool state)
    {
        if (mainMenu != null) mainMenu.SetActive(state);
        if (optionsMenu != null) optionsMenu.SetActive(!state);
    }

    public void ActivateCreditsMenu(bool _)
    {
        // Was hard-wired to a debug scene ("Level 3 Testing") that is not in the build.
        SceneManager.LoadScene(GameScenes.Credits);
    }

    public void Play()
    {
        if (loadingScenePanel != null) loadingScenePanel.SetActive(true);
        if (mainMenu != null) mainMenu.SetActive(false);
        string scene = string.IsNullOrEmpty(firstLevelScene) ? GameScenes.Level1 : firstLevelScene;
        SceneManager.LoadSceneAsync(scene);
    }

    public void Quit() => Application.Quit();

    public void LoadSceneByName(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
            SceneManager.LoadScene(sceneName);
    }
}
