using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateFour : BossStateBase
{
    private StateFourLogic fourLogic;
    public StateFour(bool needsExitTime, Player_controler player, FinalBossLevelTwo boss, StateFourLogic stateOneLogic) : base(needsExitTime, player, boss)
    {
        fourLogic = stateOneLogic;
        fourLogic.player = player;
    }

    public override void OnLogic()
    {
        base.OnLogic();
        if (RequestedExit)
        {
            fsm.StateCanExit();
            fourLogic.isEnabledThisState = false;
        }
        if (fourLogic.attack)
        {
            animator.SetBool("AttackPhase4", true);
        }
        else if (!fourLogic.attack)
        {
            animator.SetBool("AttackPhase4", false);
        }
        if (fourLogic.rest)
        {
            animator.SetBool("RestPhase4", true);

        }
        else if (!fourLogic.rest)
        {
            animator.SetBool("RestPhase4", false);

        }

    }
    public override void OnEnter()
    {
        base.OnEnter();
        animator.SetTrigger("TransitionPhase4");

        //animator.SetTrigger("StartBossBattle");
        fourLogic.StartPhase4();
        //monoBehaviour.StartCoroutine(ShootInIntervals());

    }
}
