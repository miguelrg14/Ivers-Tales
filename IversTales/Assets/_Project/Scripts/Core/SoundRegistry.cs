using System;
using System.Collections.Generic;

namespace IversTales.Core
{
    /// <summary>
    /// Maps sound identifiers to clips. Built from the list edited in the inspector;
    /// duplicated keys keep the first clip and are reported instead of throwing
    /// (the original code checked for duplicated clips, not keys, so a repeated
    /// key crashed the AudioManager on Awake).
    /// </summary>
    public sealed class SoundRegistry<TKey, TClip> where TClip : class
    {
        readonly Dictionary<TKey, TClip> clips = new Dictionary<TKey, TClip>();
        readonly List<TKey> duplicates = new List<TKey>();

        public IReadOnlyList<TKey> DuplicatedKeys => duplicates;
        public int Count => clips.Count;

        public SoundRegistry(IEnumerable<KeyValuePair<TKey, TClip>> entries)
        {
            if (entries == null) throw new ArgumentNullException(nameof(entries));
            foreach (var entry in entries)
            {
                if (entry.Value == null) continue;
                if (clips.ContainsKey(entry.Key)) { duplicates.Add(entry.Key); continue; }
                clips.Add(entry.Key, entry.Value);
            }
        }

        public bool TryGet(TKey key, out TClip clip) => clips.TryGetValue(key, out clip);
    }
}
