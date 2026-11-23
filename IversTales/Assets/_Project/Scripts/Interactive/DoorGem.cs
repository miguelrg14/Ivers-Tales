using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Gem that opens one or two <see cref="DoorSwitch"/> doors when the player picks
/// it up. Optional children: light (0), gem mesh (1) and the disappear
/// particle effect (2).
/// </summary>
public class DoorGem : MonoBehaviour
{
    [FormerlySerializedAs("puerta1")] public DoorSwitch firstDoor;
    [FormerlySerializedAs("puerta2")] public DoorSwitch secondDoor;

    GameObject lightObject;
    GameObject gemObject;
    GameObject disappearEffect;
    ParticleSystem disappearParticles;
    bool collected;

    void Awake()
    {
        lightObject = ChildOrNull(0);
        gemObject = ChildOrNull(1);
        disappearEffect = ChildOrNull(2);
        if (disappearEffect != null)
            disappearParticles = disappearEffect.GetComponent<ParticleSystem>();
    }

    GameObject ChildOrNull(int index) =>
        index < transform.childCount ? transform.GetChild(index).gameObject : null;

    void OnTriggerEnter(Collider other)
    {
        if (collected || other.GetComponentInParent<PlayerController>() == null) return;
        collected = true;

        AudioManager.Instance?.PlayClip(SoundsFX.SFX_Gem_Pickup);
        if (firstDoor != null) firstDoor.Off();
        if (secondDoor != null) secondDoor.Off();

        if (gemObject != null) gemObject.SetActive(false);
        if (lightObject != null) lightObject.SetActive(false);
        if (disappearEffect != null) disappearEffect.SetActive(true);
    }

    void Update()
    {
        // Hide the whole gem once the disappear effect has finished.
        if (collected && (disappearParticles == null || !disappearParticles.IsAlive()))
            gameObject.SetActive(false);
    }
}
