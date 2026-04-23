using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableBossL2 : MonoBehaviour
{
    FinalBossLevelTwo finalBossLevelTwo;
    // Start is called before the first frame update
    void Start()
    {
        finalBossLevelTwo=FindObjectOfType<FinalBossLevelTwo>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("heyy");
        skill_system player= other.gameObject.GetComponentInParent<skill_system>( );
        if (player != null)
        {
            finalBossLevelTwo.isEnabledBoss = true;
            player.isLevel2BossFight = true;
        }
    }
}
