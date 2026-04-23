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
using UnityEngine;
using UnityEngine.AI;
using Blackboard = RenownedGames.AITree.Blackboard;

[RequireComponent(typeof(BehaviourRunner))]
[DefaultExecutionOrder(2)]
public class ShortDistanceEnemy : Enemy
{
    private ParticleSystem fireballPS;

    [Group("Attack Configs")]
    [SerializeField]
    [Range(0f, 5f)]
    private float meleeCooldowm;
    [Group("Attack Configs")]
    public float damage  = 10f;
    public PointForMeleeUnits pointMelee { get; private set; }

    [Group("One Enemy Mode")]
    [SerializeField]
    float stoppingDistanceOneEnemyMode=3;

    [Group("Debug")]
    [SerializeField]
    bool oneEnemyMode;


    [Group("Debug Lerp Position")]
    [SerializeField]
    bool isLerpAttacking = false;
    [Group("Debug Lerp Position")]
    [SerializeField]
    float timeCooldowm = 0;

    [Group("Attack Configs")]
    [SerializeField]
    private Vector3 lastPositionForLerp;
    [Group("Attack Configs")]
    [SerializeField]
    public bool firstTimeAttack;

    void Awake()
    {
        SetReferences();
    }
    protected override void SetReferences()
    {
        base.SetReferences();
        fireballPS = GetComponentInChildren<ParticleSystem>();
        if (useWander)
        {
            isWandering = true;
        }
    }

    void Update()
    {

        if (!behaviourRunner)
        {
            return;

        }
        else
        {
            if (!isLerpAttacking)
                SetValuesBlackBoard();
            else
                RotationAttacking();
        }

    }
    void SetValuesBlackBoard()
    {

        Blackboard blackboard = behaviourRunner.GetBlackboard();
        if (useMovementPrediction)
        {

            if (player.IsPlayerMoving())
            {
                actualTargetPosition = player.transform.position + player.ReturnAverageVelocity() * movementPredictionTime;
                SetBlackBoardValue(blackboard, "PlayerPosition", actualTargetPosition);
                //if the player is moving, then uses movement prediction
            }
            else
            {
                actualTargetPosition = player.transform.position;
                SetBlackBoardValue(blackboard, "PlayerPosition", actualTargetPosition);
            }


        }
        else
        {
            actualTargetPosition = player.transform.position;
            SetBlackBoardValue(blackboard, "PlayerPosition", actualTargetPosition);

        }

        if (isInAttackRange)
        {
            if (!oneEnemyMode)   //if 2 or more enemies are following the player, then the mode 
            {
                if (player.IsPlayerMoving())
                {
                    if (pointMelee != null)
                    {
                        pointMelee.DeleteEnemy();
                        pointMelee = null;
                    }


                }
                SetBlackBoardValue(blackboard, "AcceptableRadiusMelee", 0);
                SetCircleTargetPosition();

            }
            else
            {
                if (firstTimeAttack)
                {
                    SetBlackBoardValue(blackboard, "AcceptableRadiusMelee", stoppingDistanceOneEnemyMode);

                    SetBlackBoardValue(blackboard, "CircleTargetPosition", player.transform.position);   //actualices the circletargetposition
                }
                else
                {
                    if (player.IsPlayerMoving())
                    {
                        SetBlackBoardValue(blackboard, "AcceptableRadiusMelee", 0); //sets the radius to 0 for still following the player and not being set to 0

                        SetBlackBoardValue(blackboard, "CircleTargetPosition", player.transform.position);   //actualices the circletargetposition value  with the playerPosition


                    }
                    else
                    {
                        SetBlackBoardValue(blackboard, "AcceptableRadiusMelee", stoppingDistanceOneEnemyMode);

                        SetBlackBoardValue(blackboard, "CircleTargetPosition", player.transform.position);   //actualices the circletargetposition value  with the point melee
                    }
                }




            }



        }

        SetBlackBoardValue(blackboard, "Player", player.transform);
        if (useWander)
        {
            SetBlackBoardValue(blackboard, "IsWander", isWandering);

        }
        SetBlackBoardValue(blackboard, "IsInChaseRange", isInChaseRange);
        SetBlackBoardValue(blackboard, "IsInMeleeRange", isInAttackRange);
        CanMelee(blackboard);
    }
    #region Sensors Events
    protected override void AttackSensorOffOnPlayerDetection(Vector3 lastKnownPosition)
    {
        base.AttackSensorOffOnPlayerDetection(lastKnownPosition);
        firstTimeAttack = false;

        AIManager.Instance.unitsInMeleeRange.Remove(this);  //find something with less performance impact
        if (pointMelee!=null)
        {
            pointMelee.DeleteEnemy();
            pointMelee = null;
            //removes this unit in the units in melee range, and deletes the enemy in the pointMelePosition. Also the refernce to the point melee in this script is also 
            //pointed to null
        }

    }
    protected override void AttackSensorOnOnPlayerDetection(Transform player)
    {
        base.AttackSensorOnOnPlayerDetection(player);
        if (!isDead) 
        {
            firstTimeAttack = true;
            AIManager.Instance.unitsInMeleeRange.Add(this); //adds the unit to the manager instance units in melee range
        }

    }
    #endregion
    #region Messages From Tree
    protected override void OnAttack()
    {
        base.OnAttack();
        animator.SetTrigger("Attack");
        StartCoroutine(AttackLerp());

        //timeCooldowm = 0;

    }
    #endregion
    #region Melee Circle Methods
    public void SetPositionMeleePoint(PointForMeleeUnits point)
    {
        pointMelee = point; //sets its pointMelee
        pointMelee.SetEnemy(this);  //sets the enemy to the point
    }
    public void SetCircleTargetPosition()
    {
        Blackboard blackboard = behaviourRunner.GetBlackboard();
        if (pointMelee!=null)
            SetBlackBoardValue(blackboard, "CircleTargetPosition", pointMelee.ReturnPositionPoint());   //actualizases the circletargetposition value  with the point melee
    }
    #endregion
    void RotationAttacking()
    {
        Vector3 direction = (player.transform.position - transform.position).normalized;  //follows the target
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        targetRotation.x = 0;
        targetRotation.z = 0;

        transform.rotation = targetRotation;
    }
    void CanMelee(Blackboard blackboard)
    {
        if (oneEnemyMode)
        {
            if (firstTimeAttack) 
            {
                SetBlackBoardValue(blackboard, "CanMelee", true); //if the time for the next melee attack is passed then, the enemy attacks 

            }
            else
            {
                timeCooldowm += Time.deltaTime;
                if (timeCooldowm <= meleeCooldowm)
                {
                    SetBlackBoardValue(blackboard, "CanMelee", false);

                }
                else if (player.IsPlayerMoving())
                {
                    timeCooldowm = 0;

                }
                else
                {
                    SetBlackBoardValue(blackboard, "CanMelee", true); //if the time for the next melee attack is passed then, the enemy attacks 


                }
            }
        }
        else
        {
            if (pointMelee != null)   //check if point melee is not null
            {
                if (pointMelee.ReturnIfEnemyHasReachedPoint())  //check if enemy has reached the position for enabling attack
                {
                    timeCooldowm += Time.deltaTime;
                    if (timeCooldowm <= meleeCooldowm)
                    {
                        SetBlackBoardValue(blackboard, "CanMelee", false);

                    }
                    else if (player.IsPlayerMoving())
                    {
                        timeCooldowm = 0;
                    }
                    else
                    {
                        SetBlackBoardValue(blackboard, "CanMelee", true); //if the time for the next melee attack is passed then, the enemy attacks 


                    }

                }
            }
        }


    }

    private IEnumerator AttackLerp()
    {
        if (firstTimeAttack)
        {
            audioSource.PlayOneShot(attack_clip);

            isLerpAttacking = true;
            agent.ResetPath();
            agent.enabled = false;
            behaviourRunner.enabled = false;
            agent.updatePosition = false;
            agent.updateRotation = false;

            yield return new WaitForSeconds(1f);
            if (isInAttackRange)
                player.OnDamageFromShortDistanceEnemy(this);
            yield return new WaitForSeconds(1f);

            agent.enabled = true;
            behaviourRunner.enabled = true;
            agent.updatePosition = true;
            agent.updateRotation = true;
            isLerpAttacking = false;
            timeCooldowm = 0;
            //firstTimeAttack = false;
        }
        else
        {
            if (timeCooldowm < meleeCooldowm)
            {

            }
            else
            {
                audioSource.PlayOneShot(attack_clip);

                isLerpAttacking = true;
                agent.ResetPath();
                agent.enabled = false;
                behaviourRunner.enabled = false;
                agent.updatePosition = false;
                agent.updateRotation = false;

                yield return new WaitForSeconds(1f);
                if (isInAttackRange)
                    player.OnDamageFromShortDistanceEnemy(this);
                yield return new WaitForSeconds(1f);

                agent.enabled = true;
                behaviourRunner.enabled = true;
                agent.updatePosition = true;
                agent.updateRotation = true;
                isLerpAttacking = false;
                timeCooldowm = 0;
            }
        }

    }
    public override void RecieveDamage(int damage)
    {
        base.RecieveDamage(damage);

        if (health == 0)
        {
            isDead = true;
            AIManager.Instance.unitsInMeleeRange.Remove(this);  //find something with less performance impact

            audioSource.PlayOneShot(death_clip);

            StopCoroutine(AttackLerp());

            vfxManager.ActivateVFXEnemyDie(this.transform.position, Quaternion.identity);
            behaviourRunner.enabled = false;
            agent.enabled = false;
            fireballPS.Stop();
            animator.SetTrigger("Die");
            disolveDeadEffect.StartDissableDeadEffect(this.gameObject);
            this.enabled = false;
        }
    }
    #region Getters and Setters
    public void SetOneEnemyMode(bool status)
    {

        oneEnemyMode = status;
        player.ActivateDesactivateNavMeshObstacle(status);

    }
    public bool GetOneEnemyMode()
    {
        return oneEnemyMode;
    }
    #endregion
}
