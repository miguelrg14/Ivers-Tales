using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    PlayerInput playerInput;
    //CameraController cameraController;
    //PlayerStats stats = null;
    [SerializeField] MusicManager musicManager;
    [SerializeField] UI_AudioManager ui_AudioManager;

    GameObject playerObject;

    bool isPaused = false;

    //PUT BY LUCAS
    [SerializeField] GameObject disclaimerScreen = null;
    [SerializeField] GameObject gainedNewSpellScreen = null;
    [SerializeField] Animator gainedNewSpellScreenAnimator = null;

    [SerializeField] GameObject welcomeScreen = null;

    [SerializeField] GameObject hudCanvas = null;
    [SerializeField] GameObject pauseCanvas = null;
    [SerializeField] GameObject pauseMainCanvas = null;
    [SerializeField] GameObject optionsCanvas = null;
    [SerializeField] GameObject endCanvas = null;
    [SerializeField] GameObject loadingScreenPanel;

    void Awake()
    {
        Cursor.visible = false;
    }
    
    void Start()
    {
        //Time.timeScale = 1f;
        disclaimerScreen.SetActive(false);
        welcomeScreen.SetActive(false);

        Time.timeScale = 1f;

        playerInput = new PlayerInput(); // Initialize Player input to use new input system
        playerInput.Enable(); // Enable input system!
        playerInput.Player.Menu.performed += e => ActivateIngameMenu();   // Jump

        playerObject = gameObject;
        SetActiveHud(true);
        if (playerObject != null) { }
        bool isTargetScene = CheckIfSceneIsLoaded("Level3Testing");

        if (isTargetScene)
            StartCoroutine(ActivateGainedNewSpellScreen());
        else
            StartCoroutine(ActivateDisclaimerScreen());

    }

    //void Update()   => Time.timeScale = isPaused ? 0 : 1;
    IEnumerator ActivateGainedNewSpellScreen()
    {
        yield return new WaitForSeconds(0.8f);
        gainedNewSpellScreen.SetActive(true);
        Cursor.visible = true;
        isPaused = true;
        Time.timeScale = 0f;
    }
    IEnumerator ActivateDisclaimerScreen()
    {
        yield return new WaitForSeconds(0.8f);
        disclaimerScreen.SetActive(true);
        Cursor.visible = true;
        isPaused = true;
        Time.timeScale = 0f;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (disclaimerScreen.activeInHierarchy)
            {
                WelcomeScreenActivate();
            }
            else if (welcomeScreen.activeInHierarchy)
            {
                ActivateGame();
            }
        }
    }
    public void ActivateIngameMenu()
    {
        if (!isPaused)
        {

            Cursor.visible = true;
            SetActivePause(true);
            Time.timeScale = 0f;
            //CameraController.cameraLocked = true;
        }
        else if (isPaused)
        {

            if (disclaimerScreen.activeInHierarchy ||welcomeScreen.activeInHierarchy)
            {

            }
            else
            {
                Cursor.visible = false;
                SetActivePause(false);
                Time.timeScale = 1f;
                //CameraController.cameraLocked = false;
            }

        }        
    }

    //public void ActivateIngameMenu()
    //{
    //    //if (!stats.IsDead())
    //    //{
    //    //    if (!isPaused)
    //    //    {
    //    //        SetActivePause(true);
    //    //        Time.timeScale = 0f;
    //    //        CameraController.cameraLocked = true;
    //    //    }
    //    //    else if (isPaused)
    //    //    {
    //    //        SetActivePause(false);
    //    //        Time.timeScale = 1f;
    //    //        CameraController.cameraLocked = false;
    //    //    }
    //    //}
    //}
    public void WelcomeScreenActivate()
    {
        disclaimerScreen.SetActive(false);
        welcomeScreen.SetActive(true);

    }
    public void ActivateGame()
    {
        Cursor.visible = false;
        bool isTargetScene = CheckIfSceneIsLoaded("Level3Testing");

        if (isTargetScene)
            gainedNewSpellScreen.SetActive(false);
        disclaimerScreen.SetActive(false);
        welcomeScreen.SetActive(false);
        SetActivePause(false);
        Time.timeScale = 1f;
        //CameraController.cameraLocked = false;
    }

    void SmoothlyChangeTime(float initialTime, float finalTime)
    {
        Time.timeScale = Mathf.Lerp(initialTime, finalTime, Time.smoothDeltaTime);
    }

    public void SetActiveHud(bool state)    // Playing game
    {
        hudCanvas.SetActive(state);
        endCanvas.SetActive(!state);
        optionsCanvas.SetActive(!state);

        //if (!stats.IsDead())        pauseCanvas.SetActive(!state);
    }

    public void SetActivePause(bool state)  // Pausing ingame
    {
        hudCanvas.SetActive(!state);
        pauseCanvas.SetActive(state);
        pauseMainCanvas.SetActive(state);
        optionsCanvas.SetActive(!state);

        //if (state)  cameraController.UnlockCursor();
        //else        cameraController.LockCursor();

        isPaused = state;
        //ui_AudioManager.Play_EnterMenu_Sound();
        //musicManager.LowPassFilter_SetActive(state); // Audio music plug effect
    }
    public void SetActiveOptionsMenu(bool state)  // Pausing ingame
    {
        pauseMainCanvas.SetActive(false);
        optionsCanvas.SetActive(true);
    }

    public bool GameIsPaused() => isPaused;
    public void Restart()
    {
        pauseCanvas.SetActive(false);
        loadingScreenPanel.SetActive(true);
        playerInput.Disable();
        playerObject.SetActive(false);
        SceneManager.LoadSceneAsync(1);
        Time.timeScale = 1f;
        playerObject.SetActive(true);
    }
    public void MainMenu()
    {
        pauseCanvas.SetActive(false);
        loadingScreenPanel.SetActive(true);
        playerInput.Disable();
        playerObject.SetActive(false);
        SceneManager.LoadSceneAsync(0);
        Time.timeScale = 1f;
        playerObject.SetActive(true);
    }

    bool CheckIfSceneIsLoaded(string sceneName)
    {
        int sceneCount = SceneManager.sceneCount;

        for (int i = 0; i < sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name == sceneName)
            {
                return true;
            }
        }

        return false;
    }

    public void Quit() => Application.Quit();

    void OnDisable()
    {
        playerInput.Disable();
    }
}
