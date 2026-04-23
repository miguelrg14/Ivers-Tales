using RenownedGames.AITree.Demo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateFive : BossStateBase
{
    private StateFiveLogic fiveLogic;
    public StateFive(bool needsExitTime, Player_controler player, FinalBossLevelTwo boss, StateFiveLogic stateOneLogic) : base(needsExitTime, player, boss)
    {
        fiveLogic = stateOneLogic;
    }

    public override void OnLogic()
    {
        base.OnLogic();
        if (RequestedExit)
        {
            fsm.StateCanExit();
            fiveLogic.isEnabledThisState = false;
        }

    }
    public override void OnEnter()
    {
        base.OnEnter();
        animator.SetTrigger("DieBossL2");

        //animator.SetTrigger("StartBossBattle");
        fiveLogic.StartPhase5();
        //monoBehaviour.StartCoroutine(ShootInIntervals());

    }
}
