using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityHFSM;

public class StateTwo : BossStateBase
{
    private StateTwoLogic twoLogic;

    public StateTwo(bool needsExitTime, Player_controler player, FinalBossLevelTwo boss, StateTwoLogic stateTwoLogic) : base(needsExitTime, player, boss)
    {
        twoLogic = stateTwoLogic;

    }
    public override void OnLogic()
    {
        base.OnLogic();
        if (RequestedExit)
        {
            fsm.StateCanExit();
            twoLogic.isEnabledThisState = false;
            twoLogic.DisableForceField();
        }
        if (twoLogic.shoot)
        {
            animator.SetBool("AttackTornado", true);
        }
        else if (!twoLogic.shoot)
        {
            animator.SetBool("AttackTornado", false);
        }

    }
    public override void OnEnter()
    {
        base.OnEnter();

        twoLogic.StartShooting();
        //monoBehaviour.StartCoroutine(ShootInIntervals());

    }
}
