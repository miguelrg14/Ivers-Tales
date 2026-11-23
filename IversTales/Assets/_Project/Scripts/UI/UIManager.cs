using System.Collections;
using IversTales.Core;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// In-game UI: HUD, pause menu, options and the intro screens (disclaimer and
/// welcome in level 1, "new spell" in level 2). The rules live in
/// <see cref="PauseMenuStateMachine"/>; this component only shows the panels and
/// sets the time scale and cursor for the current state.
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("Intro screens")]
    [SerializeField] GameObject disclaimerScreen = null;
    [SerializeField] GameObject gainedNewSpellScreen = null;
    [SerializeField] GameObject welcomeScreen = null;
    [SerializeField] float introDelay = 0.8f;

    [Header("Canvases")]
    [SerializeField] GameObject hudCanvas = null;
    [SerializeField] GameObject pauseCanvas = null;
    [SerializeField] GameObject pauseMainCanvas = null;
    [SerializeField] GameObject optionsCanvas = null;
    [SerializeField] GameObject endCanvas = null;
    [SerializeField] GameObject loadingScreenPanel;

    [Header("Input")]
    [Tooltip("Key that confirms the intro screens.")]
    [SerializeField] KeyCode confirmIntroKey = KeyCode.X;

    readonly PauseMenuStateMachine menu = new PauseMenuStateMachine();
    PlayerInput playerInput;

    public bool GameIsPaused() => menu.IsPaused;

    void Awake()
    {
        menu.StateChanged += (_, __) => Refresh();
        playerInput = new PlayerInput();
        playerInput.Player.Menu.performed += OnMenuPressed;
    }

    void OnEnable() => playerInput.Enable();
    void OnDisable() => playerInput.Disable();

    void OnDestroy()
    {
        playerInput.Player.Menu.performed -= OnMenuPressed;
        playerInput.Dispose();
    }

    void Start()
    {
        if (endCanvas != null) endCanvas.SetActive(false);
        Refresh();
        StartCoroutine(ShowIntroAfterDelay(GameScenes.StartsWithNewSpellScreen(SceneManager.GetActiveScene().name)));
    }

    void Update()
    {
        if (menu.IsShowingIntro && Input.GetKeyDown(confirmIntroKey))
            menu.ConfirmIntro();
    }

    IEnumerator ShowIntroAfterDelay(bool isNewSpellLevel)
    {
        yield return new WaitForSecondsRealtime(introDelay);
        if (menu.State == PauseMenuState.Playing)
            menu.ShowIntro(isNewSpellLevel);
    }

    void OnMenuPressed(InputAction.CallbackContext _) => menu.TogglePause();

    // ---- Methods called by buttons (UnityEvents in the scenes) ----

    public void ActivateIngameMenu() => menu.TogglePause();

    /// <summary>"Continue" on the disclaimer screen.</summary>
    public void WelcomeScreenActivate() => menu.ConfirmIntro();

    /// <summary>"Play" on the welcome / new spell screens and "Resume" in the pause menu.</summary>
    public void ActivateGame() => menu.ForcePlaying();

    public void SetActivePause(bool paused)
    {
        if (paused) menu.ShowPauseMenu();
        else menu.Resume();
    }

    public void SetActiveOptionsMenu(bool _) => menu.OpenOptions();

    public void Restart() => LoadScene(SceneManager.GetActiveScene().name);

    public void MainMenu() => LoadScene(GameScenes.MainMenu);

    public void Quit() => Application.Quit();

    void LoadScene(string sceneName)
    {
        if (pauseCanvas != null) pauseCanvas.SetActive(false);
        if (loadingScreenPanel != null) loadingScreenPanel.SetActive(true);
        playerInput.Disable();
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync(sceneName);
    }

    void Refresh()
    {
        var state = menu.State;
        Time.timeScale = menu.TimeScale;
        Cursor.visible = menu.IsPaused;

        SetActive(hudCanvas, state == PauseMenuState.Playing);
        SetActive(pauseCanvas, state == PauseMenuState.Paused || state == PauseMenuState.Options);
        SetActive(pauseMainCanvas, state == PauseMenuState.Paused);
        SetActive(optionsCanvas, state == PauseMenuState.Options);
        SetActive(disclaimerScreen, state == PauseMenuState.Disclaimer);
        SetActive(welcomeScreen, state == PauseMenuState.Welcome);
        SetActive(gainedNewSpellScreen, state == PauseMenuState.NewSpell);
    }

    static void SetActive(GameObject target, bool active)
    {
        if (target != null && target.activeSelf != active) target.SetActive(active);
    }
}
