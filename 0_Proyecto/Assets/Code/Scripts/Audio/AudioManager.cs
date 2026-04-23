using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     
///     Example to use:         AudioManager.instance.PlayClip(SoundsFX.soundName);
/// </summary>

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }

    AudioSource audioSource;

    //[SerializeField] AudioClip[] audioClips;
    [SerializeField]
    private List<AudioHolder> audioClips = new List<AudioHolder>();
    private Dictionary<SoundsFX, AudioClip> audioClipsDictionary = new Dictionary<SoundsFX, AudioClip>();

    [Header("Modifiers")]
    [SerializeField][Range(0f, 3f)] float pitchChangerMin = 0.5f;
    [SerializeField][Range(0f, 3f)] float pitchChangerMax = 1.8f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        audioSource = GetComponent<AudioSource>();

        foreach (AudioHolder audio in audioClips)
        {
            if (!audioClipsDictionary.ContainsValue(audio.audioClip))
                audioClipsDictionary.Add(audio.audiokey, audio.audioClip);
        }
    }

    public void PlayClip(SoundsFX audioClipKey)
    {
        if (audioClipsDictionary.ContainsKey(audioClipKey))
        {
            audioSource.PlayOneShot(audioClipsDictionary[audioClipKey]);
        }
    }

    // Modifications
    public void RandomizePitch() => audioSource.pitch = Random.Range(pitchChangerMin, pitchChangerMax);
    public void RandomizePitch(AudioSource audioSource) => audioSource.pitch = Random.Range(pitchChangerMin, pitchChangerMax);
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
public enum SoundsFX
{
    SFX_Gem_Pickup,
    SFX_Player_Attack,
    SFX_Player_Dash,
    SFX_Player_Hurt,
    SFX_Player_Hability,
    SFX_FireContact
}
