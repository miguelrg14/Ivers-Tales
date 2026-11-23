using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StateThreeLogic : MonoBehaviour
{
    [Header("Atributtes")]
    [SerializeField]
    public bool isEnabledThisState = true;
    public bool startBattle = false;
    [SerializeField]
    float projectileSpeed = 5f;
    [SerializeField]
    float shootInterval = 1f;
    [SerializeField]
    int shotsBeforePause = 3;
    [SerializeField]
    public float pauseDuration = 3f;
    [SerializeField]
    public float dashForceFieldValue = 40;
    [Header("Atributtes Fan")]
    [SerializeField]
    float initialAngle = -45f;
    [SerializeField]
    float finalAngle = 45f;
    public bool attackR { get; private set; } = false;
    public bool attackL { get; private set; } = false;

    public bool rest { get; private set; } = false;
    public PlayerController player { private get; set; }

    //Vector3 initialPosition;
    [Header("References")]
    [SerializeField]
    private BulletObjectPooling bulletObjectPooling;
    [SerializeField]
    Animator obstaclesAnimator;
    [SerializeField]
    Animator torchesAnimator;
    [SerializeField]
    GameObject rightHand;
    [SerializeField]
    GameObject leftHand;
    private VFXManager vfxManager;
    private void Awake()
    {
        vfxManager=FindObjectOfType<VFXManager>();
        //rightHand=transform.GetChild(0).GetChild(0).gameObject;
        //leftHand = transform.GetChild(0).GetChild(1).gameObject;

    }
    public void StartPhase3()
    {
        obstaclesAnimator.SetTrigger("EnableState3");
        torchesAnimator.SetTrigger("EnableState3");
        StartCoroutine(AttacksPhase3());
        //initialPosition = transform.position;
        SetVFXWhenWallsAreDown();
    }
    void SetVFXWhenWallsAreDown()
    {
        foreach (Transform child in obstaclesAnimator.transform)
        {
            vfxManager.ActivateVFXWallsDisappearBossL2(child.transform.position, Quaternion.identity);

        }

        foreach (Transform child in torchesAnimator.transform)
        {
            vfxManager.ActivateVFXWallsDisappearBossL2(child.transform.position, Quaternion.identity);
        }
    }
    void ShootFan(int shotsCount)
    {
        float value = 10f;

        if (shotsCount % 2 == 0)
        {
            // Even round: shoot the balls at different spots
            for (float angle = initialAngle; angle <= finalAngle; angle += value)
            {
                Vector3 direction = Quaternion.Euler(0f, angle, 0f) * transform.forward;
                FireProjectile(direction);
            }
        }
        else
        {
            // Odd round: shoot the balls at different spots
            for (float angle = initialAngle + value / 2; angle <= finalAngle - value / 2; angle += value)
            {
                Vector3 direction = Quaternion.Euler(0f, angle, 0f) * transform.forward;
                FireProjectile(direction);
            }
        }
    }
    void FireProjectile(Vector3 direction)
    {
        GameObject projectile = bulletObjectPooling.GetPooledObject();
        projectile.transform.position = new Vector3(transform.position.x,
            transform.position.y + 1, transform.position.z);
        projectile.transform.rotation = Quaternion.LookRotation(direction);
        projectile.SetActive(true);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.velocity = direction * projectileSpeed;
    }
    IEnumerator AttacksPhase3()
    {
        int shotsCount = 0;
        yield return new WaitForSeconds(2f);    //for starting the battle
        //shoot = true;

        while (isEnabledThisState)
        {
            if (shotsCount == 5)
            {
                shotsCount = 0;
                rest = true;
                //transform.position = initialPosition;

                yield return new WaitForSeconds(3f);
                rest = false;

            }
            else
            {
                shotsCount++;
                //attack = false;
                attackR=false;
                attackL=false;
                //Vector3 targetPosition = player.transform.position;
                //ClosestHand();
                yield return new WaitForSeconds(1f);
                //attack = true;
                //transform.position = targetPosition;
                //string closestHand = ClosestHand();
                ClosestHand();
                ShootFan(shotsCount);

                //Debug.Log("Closest hand: " + closestHand);

                yield return new WaitForSeconds(0.2f);
            }

        }
    }
    float DistanceToHand(GameObject hand)
    {
        return Vector3.Distance(player.transform.position, hand.transform.position);
    }

    void ClosestHand()
    {
        float distanceToRightHand = DistanceToHand(rightHand);
        float distanceToLeftHand = DistanceToHand(leftHand);

        if (distanceToRightHand < distanceToLeftHand)
        {
            attackR=true;
            //attackL=false;

        }
        else
        {
            attackL = true;
            //attackR = false;

        }
    }
}
