/* ================================================================
   ----------------------------------------------------------------
   Project   :   Iver Tales
   Publisher :   IguanaGo Studios
   Developer :   Lucas García Domínguez
   ----------------------------------------------------------------
   Copyright 2023-2024 IguanaGoStudios All rights reserved.
*/

using RenownedGames.AITree;
using RenownedGames.Apex;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Group("References")]
    [SerializeField]
    protected Player_controler player;
    [Group("References")]
    [SerializeField]
    protected VFXManager vfxManager;
    [Group("References")]
    [SerializeField]
    protected PlayerDetectionSensor sensorChaseDetection;
    [Group("References")]
    [SerializeField]
    protected PlayerDetectionSensor sensorAttackDetection;
    protected Animator animator;
    protected RandomWander wander;
    protected NavMeshAgent agent;
    protected SpawnEffect disolveDeadEffect;
    protected Vector3 actualTargetPosition;
    protected BehaviourRunner behaviourRunner;

    [Group("Animation Clips")]
    [SerializeField]
    protected AnimationClip idleClip;
    [Group("Animation Clips")]
    [SerializeField]
    protected AnimationClip attackClip;

    [Group("Chase")]
    [SerializeField]
    protected bool useMovementPrediction;
    [Group("Chase")]
    [SerializeField]
    [Range(-1, 1)]
    protected float movementPredictionThreshold = 0;
    [Group("Chase")]
    [SerializeField]
    [Range(0.25f, 2f)]
    protected float movementPredictionTime = 1f;
    [Group("Chase")]
    [SerializeField]
    protected float speedChase;

    [Group("Wander")]
    [SerializeField]
    protected bool useWander;
    [Group("Wander")]
    [SerializeField]
    protected float speedWander;

    [Group("Debug")]
    [SerializeField]
    protected bool isInChaseRange;
    [Group("Debug")]
    [SerializeField]
    protected bool isWandering;
    [Group("Debug")]
    [SerializeField]
    protected bool isInAttackRange;
    [Group("Debug")]
    [SerializeField]
    protected bool canAttack;

    [Group("Audio")]
    [SerializeField]
    protected AudioSource audioSource;
    [Group("Audio")]
    [SerializeField]
    protected AudioClip attack_clip;
    [Group("Audio")]
    [SerializeField]
    protected AudioClip hurt_clip;
    [Group("Audio")]
    [SerializeField]
    protected AudioClip death_clip;

    [Group("Health")]
    [SerializeField]
    protected float health = 100;
    protected bool isDead =false;

    protected virtual void SetReferences()
    {
        behaviourRunner = GetComponent<BehaviourRunner>();
        animator = GetComponentInChildren<Animator>();
        wander = GetComponent<RandomWander>();
        agent = GetComponent<NavMeshAgent>();
        disolveDeadEffect = GetComponentInChildren<SpawnEffect>();
        SetAnimationsDurations();
        sensorChaseDetection.OnPlayerEnter += ChaseSensorOnOnPlayerDetection;
        sensorChaseDetection.OnPlayerExit += ChaseSensorOffOnPlayerDetection;

        sensorAttackDetection.OnPlayerEnter += AttackSensorOnOnPlayerDetection;
        sensorAttackDetection.OnPlayerExit += AttackSensorOffOnPlayerDetection;
        Blackboard blackboard = behaviourRunner.GetBlackboard();
        SetBlackBoardValue(blackboard, "SpeedChase", speedChase);
        SetBlackBoardValue(blackboard, "SpeedWander", speedWander);
    }

    #region SetsValues Methods

    protected void SetBlackBoardValue(Blackboard blackboard, string key, Transform transformPlayer)
    {
        if (blackboard.TryGetKey(key, out TransformKey playerTransformKey))
        {
            playerTransformKey.SetValue(transformPlayer);
        }
    }

    protected void SetBlackBoardValue(Blackboard blackboard, string key, Vector3 positionPlayer)
    {
        if (blackboard.TryGetKey(key, out Vector3Key playerPositionKey))
        {
            playerPositionKey.SetValue(positionPlayer);
        }
    }
    protected void SetBlackBoardValue(Blackboard blackboard, string key, Quaternion roatationPlayer)
    {
        if (blackboard.TryGetKey(key, out QuaternionKey playerRotationKey))
        {
            playerRotationKey.SetValue(roatationPlayer);
        }
    }
    protected void SetBlackBoardValue(Blackboard blackboard, string key, bool isInChaseRangePlayer)
    {
        if (blackboard.TryGetKey(key, out BoolKey playerChaseRangeKey))
        {
            playerChaseRangeKey.SetValue(isInChaseRangePlayer);
        }
    }
    protected void SetBlackBoardValue(Blackboard blackboard, string key, float floatEnemy)
    {
        if (blackboard.TryGetKey(key, out FloatKey enemyFloatKey))
        {
            enemyFloatKey.SetValue(floatEnemy);
        }
    }
    #endregion
    #region Sensors Events

    protected virtual void ChaseSensorOffOnPlayerDetection(Vector3 lastKnownPosition)
    {
        isInChaseRange = false;
        wander.ReCalculatePosition();
        if (useWander)
            isWandering = true;
    }
    protected virtual void ChaseSensorOnOnPlayerDetection(Transform player)
    {
        isInChaseRange = true;
        if (useWander)
            isWandering = false;
    }
    protected virtual void AttackSensorOffOnPlayerDetection(Vector3 lastKnownPosition)
    {
        isInAttackRange = false;
    }

    protected virtual void AttackSensorOnOnPlayerDetection(Transform player)
    {
        if (!isDead)
            isInAttackRange = true;
    }
    #endregion
    #region Messages From Tree
    protected virtual void OnWander()
    {
        wander.MoveToPosition();

    }
    protected virtual void OnAttack()
    {
        canAttack = true;
    }
    #endregion
    protected virtual void SetAnimationsDurations()
    {
        behaviourRunner.GetBlackboard().TryGetKey("IdleAnimationAmountTime", out FloatKey animationIdleAmountTime);
        animationIdleAmountTime.SetValue(idleClip.length);
        behaviourRunner.GetBlackboard().TryGetKey("AttackAnimationAmountTime", out FloatKey animationAttackAmountTime);
        animationAttackAmountTime.SetValue(attackClip.length);

    }
    public virtual void RecieveDamage(int damage)
    {
        
        health -= damage;
        audioSource.PlayOneShot(hurt_clip);


    }
    protected Transform ReturnPlayer()
    {
        return player.GetComponent<Transform>();
    }
}
