using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityHFSM;

public class StateOneBossL3 : BossStateBase
{
    private StateOneLogicBossL3 oneLogic;
    public StateOneBossL3(bool needsExitTime, Player_controler player, FinalBossL3 boss,
        StateOneLogicBossL3 stateOneLogic) : base(needsExitTime, player, boss)
    {
        oneLogic = stateOneLogic;
    }

    public override void OnLogic()
    {
        base.OnLogic();
        //if (RequestedExit)
        //{
        //    fsm.StateCanExit();
        //    oneLogic.isEnabledThisState = false;
        //}
        //if (oneLogic.shoot)
        //{
        //    animator.SetBool("AttackTornado", true);
        //}
        //else if (!oneLogic.shoot)
        //{
        //    animator.SetBool("AttackTornado", false);
        //}

    }
    public override void OnEnter()
    {
        base.OnEnter();
        //animator.SetTrigger("StartBossBattle");
        oneLogic.StartChasing();
        //monoBehaviour.StartCoroutine(ShootInIntervals());

    }
}
