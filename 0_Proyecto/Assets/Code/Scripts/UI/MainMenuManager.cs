using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject optionsMenu;
    [SerializeField] GameObject creditsMenu;
    [SerializeField] GameObject loadingScenePanel;
    [SerializeField] string SceneName;
    void Start()
    {
        ActivateMainMenu(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ActivateMainMenu(bool state)
    {
        mainMenu.SetActive(state);
        optionsMenu.SetActive(!state);
    }
    public void ActivateCreditsMenu(bool state)
    {
        //mainMenu.SetActive(state);
        //creditsMenu.SetActive(!state);
        //commented because it charges the old UI
        //SceneManager.LoadScene("Credits");
        SceneManager.LoadScene("Level 3 Testing");

    }

    public void Play()
    {
        loadingScenePanel.SetActive(true);
        mainMenu.SetActive(false);
        SceneManager.LoadSceneAsync(1);
    }
    public void Quit() => Application.Quit();

    public void swap_scene(string name_scene)
    {
        SceneManager.LoadScene(name_scene);
    }
}
