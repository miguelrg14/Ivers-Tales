/* ================================================================
   ----------------------------------------------------------------
   Project   :   Iver Tales
   Publisher :   IguanaGo Studios
   Developer :   Lucas García Domínguez
   ----------------------------------------------------------------
   Copyright 2023-2024 IguanaGoStudios All rights reserved.
*/

using RenownedGames.AITree;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    protected PlayerController player;
    [SerializeField]
    protected VFXManager vfxManager;
    [SerializeField]
    protected PlayerDetectionSensor sensorChaseDetection;
    [SerializeField]
    protected PlayerDetectionSensor sensorAttackDetection;
    protected Animator animator;
    protected RandomWander wander;
    protected NavMeshAgent agent;
    protected SpawnEffect disolveDeadEffect;
    protected Vector3 actualTargetPosition;
    protected BehaviourRunner behaviourRunner;

    [Header("Animation Clips")]
    [SerializeField]
    protected AnimationClip idleClip;
    [SerializeField]
    protected AnimationClip attackClip;

    [Header("Chase")]
    [SerializeField]
    protected bool useMovementPrediction;
    [SerializeField]
    [Range(-1, 1)]
    protected float movementPredictionThreshold = 0;
    [SerializeField]
    [Range(0.25f, 2f)]
    protected float movementPredictionTime = 1f;
    [SerializeField]
    protected float speedChase;

    [Header("Wander")]
    [SerializeField]
    protected bool useWander;
    [SerializeField]
    protected float speedWander;

    [Header("Debug")]
    [SerializeField]
    protected bool isInChaseRange;
    [SerializeField]
    protected bool isWandering;
    [SerializeField]
    protected bool isInAttackRange;
    [SerializeField]
    protected bool canAttack;

    [Header("Audio")]
    [SerializeField]
    protected AudioSource audioSource;
    [SerializeField]
    protected AudioClip attack_clip;
    [SerializeField]
    protected AudioClip hurt_clip;
    [SerializeField]
    protected AudioClip death_clip;

    [Header("Health")]
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
    public virtual void ReceiveDamage(int damage)
    {

        health -= damage;
        audioSource.PlayOneShot(hurt_clip);

    }
    protected Transform ReturnPlayer()
    {
        return player.GetComponent<Transform>();
    }
}
