using IversTales.Core;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

/// <summary>
/// Applies the saved volume settings. The master volume drives the AudioListener;
/// every other channel is an exposed parameter on its own AudioMixer
/// ("musicVolume" on Music, "sfxVolume" on SFX, "uiVolume" on Interface and
/// "voiceVolume" on Dialogues). Loaded mixers are found at runtime by name, so
/// scenes do not need to reference them explicitly.
/// </summary>
public static class AudioSettingsApplier
{
    static VolumeSettings settings;

    public static VolumeSettings Settings => settings ??= new VolumeSettings(PlayerPrefsSettingsStore.Instance);

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Initialize()
    {
        ApplyAll();
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    static void OnSceneLoaded(Scene scene, LoadSceneMode mode) => ApplyAll();

    public static void ApplyAll()
    {
        foreach (var channel in VolumeSettings.Channels)
            Apply(channel, Settings.Get(channel));
    }

    public static void Apply(VolumeChannel channel, float linearVolume)
    {
        if (channel == VolumeChannel.Master)
        {
            AudioListener.volume = linearVolume;
            return;
        }

        string mixerName = MixerFor(channel);
        float decibels = VolumeMath.LinearToDecibels(linearVolume);
        foreach (var mixer in Resources.FindObjectsOfTypeAll<AudioMixer>())
            if (mixer.name == mixerName)
                mixer.SetFloat(VolumeSettings.KeyFor(channel), decibels);
    }

    /// <summary>Name of the AudioMixer asset that exposes the volume of <paramref name="channel"/>.</summary>
    public static string MixerFor(VolumeChannel channel)
    {
        switch (channel)
        {
            case VolumeChannel.Music: return "Music";
            case VolumeChannel.Effects: return "SFX";
            case VolumeChannel.Interface: return "Interface";
            case VolumeChannel.Dialogue: return "Dialogues";
            default: return null;
        }
    }
}
