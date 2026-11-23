using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateFourLogic : MonoBehaviour
{
    [Header("Atributtes")]
    [SerializeField]
    public bool isEnabledThisState = true;
    public bool startBattle = false;
    public bool attack { get; private set; } = false;

    public bool rest { get; private set; } = false;
    public PlayerController player { private get;  set; }

    Vector3 initialPosition;

    private VFXManager vfxManager;

    [Header("References")]
    [SerializeField]
    private PlayerDetectionSensor damageAreaPhase4Collider;
    private void Awake()
    {
        vfxManager = FindObjectOfType<VFXManager>();
        damageAreaPhase4Collider.OnPlayerEnter += ForceFieldOnOnPlayerDetection;

        //rightHand = transform.GetChild(0).GetChild(0).gameObject;
        //leftHand = transform.GetChild(0).GetChild(1).gameObject;

    }
    public void StartPhase4()
    {
        //obstaclesAnimator.SetTrigger("EnableState3");
        //torchesAnimator.SetTrigger("EnableState3");
        StartCoroutine(AttacksPhase4());
        initialPosition = transform.position;
        //SetVFXWhenWallsAreDown();
    }
    IEnumerator AttacksPhase4()
    {
        int shotsCount = 0;
        yield return new WaitForSeconds(2f);    //for starting the battle
        //shoot = true;

        while (isEnabledThisState)
        {
            if (shotsCount == 5)
            {
                damageAreaPhase4Collider.gameObject.SetActive(false);

                shotsCount = 0;
                rest = true;
                transform.position = initialPosition;

                yield return new WaitForSeconds(5f);
                rest = false;

            }
            else
            {
                damageAreaPhase4Collider.gameObject.SetActive(false);

                shotsCount++;
                //attack = false;
                attack = false;
                Vector3 targetPosition = player.transform.position;
                yield return new WaitForSeconds(0.75f);
                //attack = true;
                damageAreaPhase4Collider.gameObject.SetActive(true);

                // Smoothly move towards the target position
                float elapsedTime = 0f;
                float duration = 0.5f; // Adjust this duration according to your preference
                Vector3 startingPos = transform.position;

                while (elapsedTime < duration)
                {
                    transform.position = Vector3.Lerp(startingPos, targetPosition, elapsedTime / duration);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                // Ensure reaching the exact target position
                transform.position = targetPosition;
                attack = true;

                //string closestHand = ClosestHand();

                //Debug.Log("Closest hand: " + closestHand);

                yield return new WaitForSeconds(0.2f);
            }

        }
    }
    protected virtual void ForceFieldOnOnPlayerDetection(Transform player)
    {
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            // Direction from the force field towards the object (pushes it away)
            Vector3 pushDirection = player.transform.position - transform.position;

            // Normalise so the force does not depend on the distance
            pushDirection.Normalize();

            playerController.ReceiveDamageFromForceField(pushDirection, 10);
        }
    }
}
