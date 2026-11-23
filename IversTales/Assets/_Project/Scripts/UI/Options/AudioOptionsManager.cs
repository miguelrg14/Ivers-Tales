using IversTales.Core;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// Audio tab of the options menu: one slider per <see cref="VolumeChannel"/>.
/// The sliders are wired from code, values are saved through
/// <see cref="VolumeSettings"/> and applied by <see cref="AudioSettingsApplier"/>.
/// </summary>
public class AudioOptionsManager : MonoBehaviour
{
    [SerializeField, FormerlySerializedAs("generalVolumeSlider")] Slider masterVolumeSlider;
    [SerializeField] Slider musicVolumeSlider;
    [SerializeField] Slider effectsVolumeSlider;
    [SerializeField, FormerlySerializedAs("uIVolumeSlider")] Slider interfaceVolumeSlider;
    [SerializeField] Slider dialogueVolumeSlider;

    VolumeSettings Settings => AudioSettingsApplier.Settings;

    void Awake()
    {
        Bind(masterVolumeSlider, VolumeChannel.Master);
        Bind(musicVolumeSlider, VolumeChannel.Music);
        Bind(effectsVolumeSlider, VolumeChannel.Effects);
        Bind(interfaceVolumeSlider, VolumeChannel.Interface);
        Bind(dialogueVolumeSlider, VolumeChannel.Dialogue);
    }

    void Start()
    {
        RefreshSliders();
        AudioSettingsApplier.ApplyAll();
    }

    // Public setters kept for UnityEvents and other scripts.
    public void SetGeneralVolume(float volume) => SetVolume(VolumeChannel.Master, volume);
    public void SetMusicVolume(float volume) => SetVolume(VolumeChannel.Music, volume);
    public void SetSfxVolume(float volume) => SetVolume(VolumeChannel.Effects, volume);
    public void SetUiVolume(float volume) => SetVolume(VolumeChannel.Interface, volume);
    public void SetDialogueVolume(float volume) => SetVolume(VolumeChannel.Dialogue, volume);

    public void ResetAudioOptions()
    {
        Settings.ResetAll();
        AudioSettingsApplier.ApplyAll();
        RefreshSliders();
    }

    void SetVolume(VolumeChannel channel, float sliderValue)
    {
        var slider = SliderFor(channel);
        float linear = slider != null && slider.maxValue > 0f ? sliderValue / slider.maxValue : sliderValue;
        Settings.Set(channel, linear);
        AudioSettingsApplier.Apply(channel, Settings.Get(channel));
        UpdateLabel(slider);
    }

    void Bind(Slider slider, VolumeChannel channel)
    {
        if (slider == null) return;
        slider.onValueChanged.AddListener(value => SetVolume(channel, value));
    }

    void RefreshSliders()
    {
        foreach (var channel in VolumeSettings.Channels)
        {
            var slider = SliderFor(channel);
            if (slider == null) continue;
            slider.SetValueWithoutNotify(Settings.Get(channel) * slider.maxValue);
            UpdateLabel(slider);
        }
    }

    static void UpdateLabel(Slider slider)
    {
        if (slider == null) return;
        var label = slider.GetComponentInChildren<TextMeshProUGUI>();
        if (label != null) label.text = VolumeMath.ToPercentLabel(slider.value, slider.maxValue);
    }

    Slider SliderFor(VolumeChannel channel)
    {
        switch (channel)
        {
            case VolumeChannel.Master: return masterVolumeSlider;
            case VolumeChannel.Music: return musicVolumeSlider;
            case VolumeChannel.Effects: return effectsVolumeSlider;
            case VolumeChannel.Interface: return interfaceVolumeSlider;
            case VolumeChannel.Dialogue: return dialogueVolumeSlider;
            default: return null;
        }
    }
}
