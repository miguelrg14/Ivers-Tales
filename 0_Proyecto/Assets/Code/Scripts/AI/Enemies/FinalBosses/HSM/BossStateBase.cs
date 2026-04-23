using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.LowLevel;
using UnityHFSM;
public abstract class BossStateBase : State<BossState, StateEvent>
{
        protected readonly FinalBoss boss;
        protected Player_controler player;
        //protected readonly NavMeshAgent Agent;
        protected readonly Animator animator;
        protected bool RequestedExit;
        protected float ExitTime;
        protected readonly Action<State<BossState, StateEvent>> onEnter;
        protected readonly Action<State<BossState, StateEvent>> onLogic;
        protected readonly Action<State<BossState, StateEvent>> onExit;
        protected readonly Func<State<BossState, StateEvent>, bool> canExit;

        public BossStateBase(bool needsExitTime,Player_controler player,
            FinalBoss boss, 
            float ExitTime = 0.1f,
            Action<State<BossState, StateEvent>> onEnter = null,
            Action<State<BossState, StateEvent>> onLogic = null,
            Action<State<BossState, StateEvent>> onExit = null,
            Func<State<BossState, StateEvent>, bool> canExit = null)
        {
            this.player = player;
            this.boss = boss;
            this.onEnter = onEnter;
            this.onLogic = onLogic;
            this.onExit = onExit;
            this.canExit = canExit;
            this.ExitTime = ExitTime;
            this.needsExitTime = needsExitTime;
            //Agent = Enemy.GetComponent<NavMeshAgent>();
            animator = boss.GetComponentInChildren<Animator>();
        }

        public override void OnEnter()
        {
            base.OnEnter();
            RequestedExit = false;
            onEnter?.Invoke(this);
        }

        public override void OnLogic()
        {
            base.OnLogic();
            if (RequestedExit && timer.Elapsed >= ExitTime)
            {
                fsm.StateCanExit();
            }
        }

        public override void OnExitRequest()
        {
            if (!needsExitTime || canExit != null && canExit(this))
            {
                fsm.StateCanExit();
            }
            else
            {
                RequestedExit = true;
            }
        }
}
