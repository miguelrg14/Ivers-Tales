using UnityEngine;

/// <summary>Collectible gem: adds one to the player's counter and disappears.</summary>
public class GemPickup : MonoBehaviour
{
    PlayerController player;
    bool collected;

    void Awake()
    {
        player = FindObjectOfType<PlayerController>();
    }

    void OnTriggerEnter(Collider other)
    {
        // Only the player collects gems (enemies and projectiles used to pick them up too).
        if (collected || other.GetComponentInParent<PlayerController>() == null) return;
        collected = true;

        AudioManager.Instance?.PlayClip(SoundsFX.SFX_Gem_Pickup);
        if (player != null) player.AddGem();
        Destroy(gameObject);
    }
}
