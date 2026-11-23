using System.Collections.Generic;

namespace IversTales.Core
{
    /// <summary>
    /// Key/value storage for player settings. The game uses a PlayerPrefs-backed
    /// implementation; tests use <see cref="InMemorySettingsStore"/>.
    /// </summary>
    public interface ISettingsStore
    {
        bool HasKey(string key);
        float GetFloat(string key, float defaultValue);
        int GetInt(string key, int defaultValue);
        void SetFloat(string key, float value);
        void SetInt(string key, int value);
    }

    /// <summary>Dictionary-backed store, useful for tests and tools.</summary>
    public sealed class InMemorySettingsStore : ISettingsStore
    {
        readonly Dictionary<string, float> floats = new Dictionary<string, float>();
        readonly Dictionary<string, int> ints = new Dictionary<string, int>();

        public bool HasKey(string key) => floats.ContainsKey(key) || ints.ContainsKey(key);
        public float GetFloat(string key, float defaultValue) => floats.TryGetValue(key, out var v) ? v : defaultValue;
        public int GetInt(string key, int defaultValue) => ints.TryGetValue(key, out var v) ? v : defaultValue;
        public void SetFloat(string key, float value) => floats[key] = value;
        public void SetInt(string key, int value) => ints[key] = value;
    }
}
