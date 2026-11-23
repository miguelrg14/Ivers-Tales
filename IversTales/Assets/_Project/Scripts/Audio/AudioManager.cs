using System.Collections.Generic;
using System.Linq;
using IversTales.Core;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Plays the shared gameplay sound effects (gem pickup, attacks, dash, hits...).
/// There is one AudioManager per scene; any script can play a sound with:
/// <code>AudioManager.Instance.PlayClip(SoundsFX.SFX_Gem_Pickup);</code>
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField]
    [FormerlySerializedAs("audioClips")]
    List<AudioHolder> clips = new List<AudioHolder>();

    [Header("Modifiers")]
    [SerializeField, Range(0f, 3f)] float pitchChangerMin = 0.5f;
    [SerializeField, Range(0f, 3f)] float pitchChangerMax = 1.8f;

    AudioSource audioSource;
    SoundRegistry<SoundsFX, AudioClip> registry;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Some scenes (credits) have no AudioSource on the manager; add one instead of failing on PlayClip.
        if (!TryGetComponent(out audioSource))
            audioSource = gameObject.AddComponent<AudioSource>();
        registry = new SoundRegistry<SoundsFX, AudioClip>(
            clips.Where(c => c != null).Select(c => new KeyValuePair<SoundsFX, AudioClip>(c.audiokey, c.audioClip)));

        foreach (var duplicated in registry.DuplicatedKeys)
            Debug.LogWarning($"{nameof(AudioManager)}: sound '{duplicated}' is assigned more than once; the first clip is used.", this);
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void PlayClip(SoundsFX key)
    {
        if (registry != null && registry.TryGet(key, out var clip))
            audioSource.PlayOneShot(clip);
    }

    public void RandomizePitch() => RandomizePitch(audioSource);
    public void RandomizePitch(AudioSource source) => source.pitch = Random.Range(pitchChangerMin, pitchChangerMax);
}

[System.Serializable]
public class AudioHolder
{
    public SoundsFX audiokey;
    public AudioClip audioClip;

    public AudioHolder(SoundsFX audiokey, AudioClip audioClip)
    {
        this.audiokey = audiokey;
        this.audioClip = audioClip;
    }
}

/// <summary>Sound identifiers. Stored by index in the scenes: append new values at the end.</summary>
public enum SoundsFX
{
    SFX_Gem_Pickup,
    SFX_Player_Attack,
    SFX_Player_Dash,
    SFX_Player_Hurt,
    SFX_Player_Ability,
    SFX_FireContact
}
