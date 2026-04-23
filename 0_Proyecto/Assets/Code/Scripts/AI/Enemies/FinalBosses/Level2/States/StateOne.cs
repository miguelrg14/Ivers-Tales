using JetBrains.Annotations;
using RenownedGames.AITree.Demo;
using RenownedGames.Apex;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityHFSM;

public class StateOne : BossStateBase
{
    private StateOneLogic oneLogic;
    public StateOne(bool needsExitTime, Player_controler player, FinalBossLevelTwo boss,StateOneLogic stateOneLogic) : base(needsExitTime,player,boss)
    {
        oneLogic = stateOneLogic;
    }

    public override void OnLogic()
    {
        base.OnLogic();
        if (RequestedExit)
        {
            fsm.StateCanExit();
            oneLogic.isEnabledThisState = false;
        }
        if (oneLogic.shoot)
        {
            animator.SetBool("AttackTornado",true);
        }
        else if (!oneLogic.shoot)
        {
            animator.SetBool("AttackTornado", false);
        }

    }
    public override void OnEnter()
    {
        base.OnEnter();
        animator.SetTrigger("StartBossBattle");
        oneLogic.StartShooting();
        //monoBehaviour.StartCoroutine(ShootInIntervals());

    }

}
