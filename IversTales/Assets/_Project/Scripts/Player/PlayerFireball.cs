using UnityEngine;

/// <summary>Fireball cast by the player: damages the first enemy it touches and disappears.</summary>
public class PlayerFireball : MonoBehaviour
{
    const int EnemyDamage = 100;
    const int BossDamage = 25;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;

        // Fireballs do not affect the level 1 boss (same behaviour as the shipped build).
        var bossLevel2 = other.GetComponentInParent<FinalBossLevelTwo>();
        if (bossLevel2 != null)
            bossLevel2.ReceiveDamage(BossDamage);
        else
        {
            var enemy = other.GetComponentInParent<Enemy>();
            if (enemy != null) enemy.ReceiveDamage(EnemyDamage);
        }

        Destroy(gameObject);
    }
}
