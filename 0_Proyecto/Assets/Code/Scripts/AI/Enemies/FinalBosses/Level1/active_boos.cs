using RenownedGames.Apex;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class active_boos : MonoBehaviour
{
    FinalBossLevelTwo finalBossLevelTwo;
    FinalBossLevelOne finalBossLevel;

    BoxCollider wallCollider;
    Animator animator;

    [Group("Audio")]
    [SerializeField]
    AudioSource levelMusic_audioSource;
    [Group("Audio")]
    [SerializeField]
    AudioSource bossMusic_audioSource;

    private void Start()
    {
        wallCollider = GetComponentInChildren<BoxCollider>();
        animator = GetComponent<Animator>();
        finalBossLevelTwo = FindObjectOfType<FinalBossLevelTwo>();
        finalBossLevel=FindObjectOfType<FinalBossLevelOne>();
    }
    private void OnTriggerEnter(Collider other)
    {
        skill_system player = other.gameObject.GetComponentInParent<skill_system>();
        if (player != null)
        {
            if (finalBossLevelTwo)
            {
                finalBossLevelTwo.isEnabledBoss = true;
                player.isLevel2BossFight = true;
            }
            else
            {
                finalBossLevel.startBattle = true;
                levelMusic_audioSource.Stop();
                bossMusic_audioSource.Play();
            }


        }
        StartCoroutine(CloseWall());
    }
    IEnumerator CloseWall()
    {
        animator.SetBool("active", true);
        yield return new WaitForSeconds(1f);
        wallCollider.isTrigger = false;
        yield return new WaitForSeconds(1.5f);

    }

}
