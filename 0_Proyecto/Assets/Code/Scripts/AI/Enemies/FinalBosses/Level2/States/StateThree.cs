using RenownedGames.AITree.Demo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateThree : BossStateBase
{
    private StateThreeLogic threeLogic;

    public StateThree(bool needsExitTime, Player_controler player, FinalBossLevelTwo boss, StateThreeLogic stateThreeLogic) : base(needsExitTime, player, boss)
    {
        threeLogic = stateThreeLogic;
        threeLogic.player = player;
    }
    public override void OnLogic()
    {
        base.OnLogic();
        if (RequestedExit)
        {
            fsm.StateCanExit();
            threeLogic.isEnabledThisState = false;
        }
        if (threeLogic.rest)
        {
            animator.SetBool("RestPhase3", true);

        }
        else if (!threeLogic.rest)
        {
            animator.SetBool("RestPhase3", false);

        }
        if (threeLogic.attackR)
        {
            animator.SetBool("AttackPhaseR3",true);

        }
        else if (!threeLogic.attackR)
        {
            animator.SetBool("AttackPhaseR3", false);

        }
        if (threeLogic.attackL)
        {
            animator.SetBool("AttackPhaseL3", true);

        }
        else if (!threeLogic.attackL)
        {
            animator.SetBool("AttackPhaseL3", false);

        }
    }
    public override void OnEnter()
    {
        base.OnEnter();
        animator.SetTrigger("TransitionPhase3");
        threeLogic.StartPhase3();
        //monoBehaviour.StartCoroutine(ShootInIntervals());

    }
}
