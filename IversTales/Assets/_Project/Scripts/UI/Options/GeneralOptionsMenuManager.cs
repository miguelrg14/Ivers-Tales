using IversTales.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>Switches between the Graphics, Controls and Audio tabs of the options menu.</summary>
public class GeneralOptionsMenuManager : MonoBehaviour
{
    [SerializeField] GameObject graphicsCanvas = null;
    [SerializeField] GameObject controlsCanvas = null;
    [SerializeField] GameObject audioCanvas = null;

    void Start() => ShowTab(graphicsCanvas);

    // The bool parameter is required by the UnityEvents already set up in the scenes.
    public void SetActiveGraphicOptions(bool _) => ShowTab(graphicsCanvas);
    public void SetActiveControlOptions(bool _) => ShowTab(controlsCanvas);
    public void SetActiveAudioOptions(bool _) => ShowTab(audioCanvas);

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(GameScenes.MainMenu);
    }

    public void Quit() => Application.Quit();

    void ShowTab(GameObject tab)
    {
        if (graphicsCanvas != null) graphicsCanvas.SetActive(tab == graphicsCanvas);
        if (controlsCanvas != null) controlsCanvas.SetActive(tab == controlsCanvas);
        if (audioCanvas != null) audioCanvas.SetActive(tab == audioCanvas);
    }
}
