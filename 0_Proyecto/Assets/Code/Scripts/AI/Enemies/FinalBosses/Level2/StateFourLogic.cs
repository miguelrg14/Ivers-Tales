using RenownedGames.Apex;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateFourLogic : MonoBehaviour
{
    [Group("Atributtes")]
    [SerializeField]
    public bool isEnabledThisState = true;
    public bool startBattle = false;
    public bool attack { get; private set; } = false;

    public bool rest { get; private set; } = false;
    public Player_controler player { private get;  set; }

    Vector3 initialPosition;

    private VFXManager vfxManager;

    [Group("References")]
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
        Debug.Log("Phase4");
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
        Player_controler player_Controler = player.GetComponent<Player_controler>();
        if (player_Controler != null)
        {
            // Calcula la dirección opuesta al vector de la posición del objeto hacia la posición del campo de fuerza
            Vector3 direccionEmpuje = player.transform.position - transform.position;

            // Normaliza la dirección para que tenga una magnitud de 1
            direccionEmpuje.Normalize();

            player_Controler.RecieveDamageFromForceField(direccionEmpuje, 10);
        }
    }
}
