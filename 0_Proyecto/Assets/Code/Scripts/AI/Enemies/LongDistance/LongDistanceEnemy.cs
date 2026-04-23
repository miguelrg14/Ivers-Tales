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
using System.Collections;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using Blackboard = RenownedGames.AITree.Blackboard;

[RequireComponent(typeof(BehaviourRunner))]
[DefaultExecutionOrder(2)]
public class LongDistanceEnemy : Enemy
{

    private RotationShootSpawnLocation shootSpawnLocation;
    [Group("References")]
    [SerializeField]
    private BulletObjectPooling bulletObjectPooling;
    private MortarShot mortar;
    [SerializeField]
    private float stoppingDistanceValueShooting;    //depends on the radius of sensorShootDetection

    [Group("Shoot Configs")]
    [SerializeField]
    [Range(0.05f, 10f)]
    private float shootCooldowm;

    [Group("Debug")]
    [SerializeField]
    float timeCooldowm = 0;
    [Group("Debug")]
    [SerializeField]
    bool firstAttack = false;
    [Group("Debug")]
    [SerializeField]


    // Start is called before the first frame update
    void Awake()
    {
        SetReferences();
    }
    protected override void SetReferences()
    {
        base.SetReferences();
        stoppingDistanceValueShooting = sensorAttackDetection.gameObject.GetComponent<SphereCollider>().radius;
        stoppingDistanceValueShooting -= 2;
        shootSpawnLocation = GetComponent<RotationShootSpawnLocation>();
        shootSpawnLocation.SetPlayer(ReturnPlayer());
        agent.stoppingDistance = stoppingDistanceValueShooting;   //its set 2 m less 
        mortar = GetComponent<MortarShot>();
        mortar.playerPosition = player.transform;
        if (useWander)
        {
            isWandering = true;
            animator.SetBool("Move", true);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (!behaviourRunner)
        {
            return;

        }
        else
        {
            SetValuesBlackBoard();
        }

    }
    void SetValuesBlackBoard()
    {
        Blackboard blackboard = behaviourRunner.GetBlackboard();
        if (useMovementPrediction)
        {
            if (player.IsPlayerMoving())
            {
                SetBlackBoardValue(blackboard, "PlayerPosition", player.transform.position + player.ReturnAverageVelocity() * movementPredictionTime);
                //if the player is moving, then uses movement prediction
            }
            else
            {
                SetBlackBoardValue(blackboard, "PlayerPosition", player.transform.position);
            }

        }
        else
        {
            SetBlackBoardValue(blackboard, "PlayerPosition", player.transform.position);

        }
        SetBlackBoardValue(blackboard, "Player", player.transform);
        if (useWander)
        {
            SetBlackBoardValue(blackboard, "IsWander", isWandering);

        }
        SetBlackBoardValue(blackboard, "IsInChaseRange", isInChaseRange);
        SetBlackBoardValue(blackboard, "IsInShootRange", isInAttackRange);
        SetBlackBoardValue(blackboard, "FirstTimeShoot", firstAttack);
        CanShoot(blackboard);

        //SetBlackBoardValue(blackboard, "AcceptableRadiusForShooting", stoppingDistanceValueShooting-2);
        //SetBlackBoardValue(blackboard, "CanShoot", lastShootTime + shootCooldowm <= Time.time); //if the time for the next melee attack is passed then, the enemy attacks 
        SetBlackBoardValue(blackboard, "ShootSpawnLocation", shootSpawnLocation.transform.position);

        //SetBlackBoardValue(blackboard, "RotationShoot", shootSpawnLocation.rotation);
    }

    #region Sensors Events

    protected override void ChaseSensorOffOnPlayerDetection(Vector3 lastKnownPosition)
    {
        base.ChaseSensorOffOnPlayerDetection(lastKnownPosition);
        animator.SetBool("Move", false);
    }
    protected override void ChaseSensorOnOnPlayerDetection(Transform player)
    {
        base.ChaseSensorOnOnPlayerDetection(player);
        agent.stoppingDistance = stoppingDistanceValueShooting;
        animator.SetBool("Move", true);

    }
    protected override void AttackSensorOffOnPlayerDetection(Vector3 lastKnownPosition)
    {
        base.AttackSensorOffOnPlayerDetection(lastKnownPosition);
        shootSpawnLocation.enabledRotation = false;
        firstAttack = false;

    }

    protected override void AttackSensorOnOnPlayerDetection(Transform player)
    {
        base.AttackSensorOnOnPlayerDetection(player);
        if (!isDead)
        {
            agent.stoppingDistance = stoppingDistanceValueShooting;
            shootSpawnLocation.enabledRotation = true;
            animator.SetBool("Move", false);
            firstAttack = true;

        }

    }
    #endregion
    #region Messages From Tree

    protected override void OnAttack()
    {
        base.OnAttack();
        timeCooldowm = 0;

        StartCoroutine(ShootAnimationSetCorrectTimes());

        audioSource.PlayOneShot(attack_clip);
    }
    protected override void OnWander()
    {
        agent.stoppingDistance = 0;
        base.OnWander();
    }
    IEnumerator ShootAnimationSetCorrectTimes()
    {

        //yield return new WaitForSeconds(0.35f);
        if (health != 0)
        {
            mortar.actualGOBullet = bulletObjectPooling.GetPooledObject();
            mortar.Launch(player.transform.position);
        }
        yield return null;

        //yield return new WaitForSeconds(0.5f);
        canAttack = false;


        animator.SetTrigger("Attack");
        if (firstAttack)
        {
            firstAttack = false;
        }
    }
    public void Move()
    {
        Vector3 positionPlayer = AIManager.Instance.ReturnNearestPoint(transform.position);

        agent.SetDestination(positionPlayer);

    }
    #endregion
    protected override void SetAnimationsDurations()
    {
        behaviourRunner.GetBlackboard().TryGetKey("IdleAnimationAmountTime", out FloatKey animationIdleAmountTime);
        animationIdleAmountTime.SetValue(idleClip.length);
        behaviourRunner.GetBlackboard().TryGetKey("ShootAnimationAmountTime", out FloatKey animationShootAmountTime);
        animationShootAmountTime.SetValue(attackClip.length);

    }
    void CanShoot(Blackboard blackboard)
    {

        timeCooldowm += Time.deltaTime;
        if (firstAttack)
        {
            SetBlackBoardValue(blackboard, "CanShoot", true); //if the time for the next melee attack is passed then, the enemy attacks 

        }
        else if (timeCooldowm <= shootCooldowm - attackClip.length)
        {
            SetBlackBoardValue(blackboard, "CanShoot", false);
        }
        else
        {
            SetBlackBoardValue(blackboard, "CanShoot", true); //if the time for the next melee attack is passed then, the enemy attacks 
            //timeCooldowm = 0;

        }

    }
    public override void RecieveDamage(int damage)
    {
        base.RecieveDamage(damage);

        if (health == 0)
        {
            isDead = true;
            audioSource.PlayOneShot(death_clip);

            //animator.enabled = false;
            //agent.enabled = false;
            //onDieExplosionLongDistanceEnemy.gameObject.SetActive(true);
            vfxManager.ActivateVFXEnemyDie(this.transform.position, Quaternion.identity);
            behaviourRunner.enabled = false;
            agent.enabled = false;
            animator.SetTrigger("Die");
            shootSpawnLocation.enabledRotation = false;
            disolveDeadEffect.StartDissableDeadEffect(this.gameObject);
            this.enabled = false;

        }
    }
}
