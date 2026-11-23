using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using IversTales.Core;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// Graphics tab of the options menu: quality, resolution, screen mode, brightness,
/// field of view, frame-rate limit and V-Sync. Controls are wired from code and
/// every value is saved in PlayerPrefs and applied when a scene starts.
/// </summary>
public class GraphicOptionsManager : MonoBehaviour
{
    [SerializeField] TMP_Dropdown qualityDropdown;
    [SerializeField] TMP_Dropdown resolutionDropdown;
    [SerializeField] TMP_Dropdown screenModeDropdown;
    [SerializeField] Slider brightnessSlider;
    [Tooltip("Full-screen black image used to darken the game.")]
    [SerializeField, FormerlySerializedAs("brightnesspanel")] Image brightnessOverlay;
    [SerializeField] Slider fovSlider;
    [SerializeField] Slider fpsLimitSlider;
    [SerializeField] TMP_Dropdown vSyncDropdown;

    const int DefaultFieldOfView = 90;
    const int DefaultFpsLimit = 240;
    const int DefaultVSync = 1;          // "Yes"
    const int DefaultScreenMode = 0;     // "Windowed Fullscreen"

    static readonly FullScreenMode[] ScreenModes =
    {
        FullScreenMode.FullScreenWindow,
        FullScreenMode.ExclusiveFullScreen,
        FullScreenMode.Windowed
    };

    readonly Dictionary<CinemachineVirtualCamera, float> authoredFieldOfView = new Dictionary<CinemachineVirtualCamera, float>();
    List<ScreenSize> resolutions = new List<ScreenSize>();

    void Awake()
    {
        if (qualityDropdown != null) qualityDropdown.onValueChanged.AddListener(SetQuality);
        if (resolutionDropdown != null) resolutionDropdown.onValueChanged.AddListener(SetResolution);
        if (screenModeDropdown != null) screenModeDropdown.onValueChanged.AddListener(SetScreenMode);
        if (vSyncDropdown != null) vSyncDropdown.onValueChanged.AddListener(EnableVSync);
        if (brightnessSlider != null) brightnessSlider.onValueChanged.AddListener(SetBrightness);
        if (fovSlider != null) fovSlider.onValueChanged.AddListener(value => SetFOV(Mathf.RoundToInt(value)));
        if (fpsLimitSlider != null) fpsLimitSlider.onValueChanged.AddListener(value => SetFpsLimit(Mathf.RoundToInt(value)));
    }

    void Start()
    {
        foreach (var vcam in FindObjectsOfType<CinemachineVirtualCamera>(true))
            authoredFieldOfView[vcam] = vcam.m_Lens.FieldOfView;

        BuildResolutionOptions();
        LoadAndApply();
    }

    // ---- Public API (also used by UnityEvents) ----

    public void SetQuality(int optionIndex)
    {
        PlayerPrefs.SetInt(GraphicsSettingsLogic.QualityKey, optionIndex);
        ApplyQuality(optionIndex);
    }

    public void SetResolution(int index)
    {
        if (index < 0 || index >= resolutions.Count) return;
        PlayerPrefs.SetInt(GraphicsSettingsLogic.ResolutionKey, index);
        var size = resolutions[index];
        Screen.SetResolution(size.Width, size.Height, Screen.fullScreenMode);
    }

    public void SetScreenMode(int index)
    {
        index = Mathf.Clamp(index, 0, ScreenModes.Length - 1);
        PlayerPrefs.SetInt(GraphicsSettingsLogic.ScreenModeKey, index);
        Screen.fullScreenMode = ScreenModes[index];
    }

    public void SetBrightness(float value)
    {
        PlayerPrefs.SetFloat(GraphicsSettingsLogic.BrightnessKey, value);
        ApplyBrightness(value);
        SetLabel(brightnessSlider, value.ToString("F2"));
    }

    public void SetFOV(int value)
    {
        PlayerPrefs.SetInt(GraphicsSettingsLogic.FieldOfViewKey, value);
        ApplyFieldOfView(value);
        SetLabel(fovSlider, value.ToString());
    }

    public void SetFpsLimit(int value)
    {
        PlayerPrefs.SetInt(GraphicsSettingsLogic.FpsLimitKey, value);
        Application.targetFrameRate = GraphicsSettingsLogic.ClampFrameRate(value);
        SetLabel(fpsLimitSlider, value.ToString());
    }

    public void EnableVSync(int value)
    {
        PlayerPrefs.SetInt(GraphicsSettingsLogic.VSyncKey, value);
        QualitySettings.vSyncCount = value > 0 ? 1 : 0;
    }

    public void ResetGraphicOptions()
    {
        PlayerPrefs.DeleteKey(GraphicsSettingsLogic.QualityKey);
        PlayerPrefs.DeleteKey(GraphicsSettingsLogic.ScreenModeKey);
        PlayerPrefs.DeleteKey(GraphicsSettingsLogic.ResolutionKey);
        PlayerPrefs.DeleteKey(GraphicsSettingsLogic.BrightnessKey);
        PlayerPrefs.DeleteKey(GraphicsSettingsLogic.FieldOfViewKey);
        PlayerPrefs.DeleteKey(GraphicsSettingsLogic.FpsLimitKey);
        PlayerPrefs.DeleteKey(GraphicsSettingsLogic.VSyncKey);

        SetScreenMode(DefaultScreenMode);
        if (resolutions.Count > 0) SetResolution(resolutions.Count - 1);
        LoadAndApply();
    }

    // ---- Internals ----

    void LoadAndApply()
    {
        int quality = PlayerPrefs.GetInt(GraphicsSettingsLogic.QualityKey, CurrentQualityOption());
        SetDropdown(qualityDropdown, quality);
        ApplyQuality(quality);

        SetDropdown(screenModeDropdown, PlayerPrefs.GetInt(GraphicsSettingsLogic.ScreenModeKey, ScreenModeIndex(Screen.fullScreenMode)));

        int vSync = PlayerPrefs.GetInt(GraphicsSettingsLogic.VSyncKey, DefaultVSync);
        SetDropdown(vSyncDropdown, vSync);
        QualitySettings.vSyncCount = vSync > 0 ? 1 : 0;

        float maxBrightness = brightnessSlider != null ? brightnessSlider.maxValue : 1f;
        float brightness = PlayerPrefs.GetFloat(GraphicsSettingsLogic.BrightnessKey, maxBrightness);
        SetSlider(brightnessSlider, brightness, brightness.ToString("F2"));
        ApplyBrightness(brightness);

        int fov = PlayerPrefs.GetInt(GraphicsSettingsLogic.FieldOfViewKey, DefaultFieldOfView);
        SetSlider(fovSlider, fov, fov.ToString());
        ApplyFieldOfView(fov);

        int fps = PlayerPrefs.GetInt(GraphicsSettingsLogic.FpsLimitKey, DefaultFpsLimit);
        SetSlider(fpsLimitSlider, fps, fps.ToString());
        Application.targetFrameRate = GraphicsSettingsLogic.ClampFrameRate(fps);
    }

    void BuildResolutionOptions()
    {
        resolutions = GraphicsSettingsLogic.DistinctResolutions(
            Screen.resolutions.Select(r => new ScreenSize(r.width, r.height)));
        if (resolutionDropdown == null) return;

        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(resolutions.Select(r => r.ToString()).ToList());
        int current = GraphicsSettingsLogic.IndexOfResolution(resolutions, new ScreenSize(Screen.width, Screen.height));
        if (current >= 0) resolutionDropdown.SetValueWithoutNotify(current);
        resolutionDropdown.RefreshShownValue();
    }

    void ApplyQuality(int optionIndex)
    {
        if (qualityDropdown == null || qualityDropdown.options.Count == 0) return;
        optionIndex = Mathf.Clamp(optionIndex, 0, qualityDropdown.options.Count - 1);
        int level = GraphicsSettingsLogic.ResolveQualityLevel(QualitySettings.names, qualityDropdown.options[optionIndex].text, optionIndex);
        QualitySettings.SetQualityLevel(level, true);
    }

    int CurrentQualityOption()
    {
        if (qualityDropdown == null) return 0;
        string current = QualitySettings.names[QualitySettings.GetQualityLevel()];
        int index = qualityDropdown.options.FindIndex(o => o.text == current);
        return index >= 0 ? index : qualityDropdown.options.Count - 1;
    }

    void ApplyBrightness(float brightness)
    {
        if (brightnessOverlay == null || brightnessSlider == null) return;
        var color = brightnessOverlay.color;
        color.a = GraphicsSettingsLogic.OverlayAlphaForBrightness(brightness, brightnessSlider.minValue, brightnessSlider.maxValue);
        brightnessOverlay.color = color;
    }

    void ApplyFieldOfView(int sliderValue)
    {
        foreach (var pair in authoredFieldOfView)
            if (pair.Key != null)
                pair.Key.m_Lens.FieldOfView = GraphicsSettingsLogic.ScaleFieldOfView(pair.Value, sliderValue, DefaultFieldOfView);
    }

    static int ScreenModeIndex(FullScreenMode mode)
    {
        int index = System.Array.IndexOf(ScreenModes, mode);
        return index >= 0 ? index : DefaultScreenMode;
    }

    static void SetDropdown(TMP_Dropdown dropdown, int value)
    {
        if (dropdown == null || dropdown.options.Count == 0) return;
        dropdown.SetValueWithoutNotify(Mathf.Clamp(value, 0, dropdown.options.Count - 1));
        dropdown.RefreshShownValue();
    }

    static void SetSlider(Slider slider, float value, string label)
    {
        if (slider == null) return;
        slider.SetValueWithoutNotify(value);
        SetLabel(slider, label);
    }

    static void SetLabel(Slider slider, string text)
    {
        if (slider == null) return;
        var label = slider.GetComponentInChildren<TextMeshProUGUI>();
        if (label != null) label.text = text;
    }
}
