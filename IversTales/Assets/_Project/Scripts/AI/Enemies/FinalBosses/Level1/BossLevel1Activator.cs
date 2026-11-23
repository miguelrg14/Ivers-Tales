using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossLevel1Activator : MonoBehaviour
{
    FinalBossLevelTwo finalBossLevelTwo;
    FinalBossLevelOne finalBossLevel;

    BoxCollider wallCollider;
    Animator animator;

    [Header("Audio")]
    [SerializeField]
    AudioSource levelMusic_audioSource;
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
        PlayerSkills player = other.gameObject.GetComponentInParent<PlayerSkills>();
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
