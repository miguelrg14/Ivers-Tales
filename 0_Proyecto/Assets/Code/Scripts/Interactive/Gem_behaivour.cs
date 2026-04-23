using RenownedGames.Apex;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem_behaivour : MonoBehaviour
{
    Player_controler Player;
    private void Awake()
    {
         Player =FindObjectOfType<Player_controler>();
    }
    private void OnTriggerEnter(Collider other)
    {
        AudioManager.instance.PlayClip(SoundsFX.SFX_Gem_Pickup);
        Player.add_gems();
        Destroy(this.gameObject);

    }
}
