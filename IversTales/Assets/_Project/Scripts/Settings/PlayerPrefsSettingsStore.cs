using IversTales.Core;
using UnityEngine;

/// <summary>
/// <see cref="ISettingsStore"/> backed by Unity's PlayerPrefs, so the options
/// survive scene changes and game restarts.
/// </summary>
public sealed class PlayerPrefsSettingsStore : ISettingsStore
{
    public static readonly PlayerPrefsSettingsStore Instance = new PlayerPrefsSettingsStore();

    PlayerPrefsSettingsStore() { }

    public bool HasKey(string key) => PlayerPrefs.HasKey(key);
    public float GetFloat(string key, float defaultValue) => PlayerPrefs.GetFloat(key, defaultValue);
    public int GetInt(string key, int defaultValue) => PlayerPrefs.GetInt(key, defaultValue);
    public void SetFloat(string key, float value) => PlayerPrefs.SetFloat(key, value);
    public void SetInt(string key, int value) => PlayerPrefs.SetInt(key, value);
}
