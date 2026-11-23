using UnityEngine;

/// <summary>Trigger at the entrance of the level 2 boss room: starts the fight.</summary>
public class EnableBossL2 : MonoBehaviour
{
    FinalBossLevelTwo finalBossLevelTwo;

    void Start()
    {
        finalBossLevelTwo = FindObjectOfType<FinalBossLevelTwo>();
    }

    void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponentInParent<PlayerSkills>();
        if (player == null || finalBossLevelTwo == null) return;

        finalBossLevelTwo.isEnabledBoss = true;
        player.isLevel2BossFight = true;
    }
}
