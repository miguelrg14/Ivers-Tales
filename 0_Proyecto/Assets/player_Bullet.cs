using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_Bullet : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy")) // Si colisiona con un enemigo
        {
            Enemy enemyController = other.GetComponentInParent<Enemy>();
            FinalBossLevelOne bossController = other.GetComponentInParent<FinalBossLevelOne>();
            FinalBossLevelTwo bossController2 = other.GetComponentInParent<FinalBossLevelTwo>();
            if (bossController2!=null)
                bossController2.RecieveDamage(25);
            else if (enemyController != null)
            {
                enemyController.RecieveDamage(100); // Llamamos a la función para causar daño al enemigo

            }
            Destroy(gameObject); // find something cheaper, f.e: a pool of player_bullets
        }

    }
}
