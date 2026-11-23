using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateTwoLogic : MonoBehaviour
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
    public bool resting { get; private set; } = false;
    public bool shoot { get; private set; } = false;

    [Header("References")]
    [SerializeField]
    private BulletObjectPooling bulletObjectPooling;
    [SerializeField]
    private PlayerDetectionSensor forceField;

    [SerializeField]
    Animator wall;
    private void Awake()
    {
        forceField.OnPlayerEnter += ForceFieldOnOnPlayerDetection;
    }
    public void DisableForceField()
    {
        forceField.gameObject.SetActive(false);
    }
    public void StartShooting()
    {
        wall.SetTrigger("EnableState2");
        StartCoroutine(ShootInIntervals());
        forceField.gameObject.SetActive(true);
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
    void OnTriggerStay(Collider other)
    {
        // Only objects with a Rigidbody can be pushed
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Direction from the force field towards the object (pushes it away)
            Vector3 pushDirection = transform.position - other.transform.position;

            // Normalise so the force does not depend on the distance
            pushDirection.Normalize();

            // Push the object away
            rb.AddForce(pushDirection * 10, ForceMode.Force);
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
    IEnumerator ShootInIntervals()
    {
        int shotsCount = 0;
        yield return new WaitForSeconds(2f);    //for starting the battle
        shoot = true;

        while (isEnabledThisState)
        {

            if (shotsCount == shotsBeforePause)
            {
                shoot = false;
                //resting = true;
                yield return new WaitForSeconds(pauseDuration);
                shotsCount = 0;
                //resting = false;
                shoot = true;

            }
            else
            {
                ShootFan(shotsCount);
                shotsCount++;
                yield return new WaitForSeconds(shootInterval);

            }

        }
    }
    protected virtual void ForceFieldOnOnPlayerDetection(Transform player)
    {
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            // Direction from the force field towards the object (pushes it away)
            Vector3 pushDirection = player.transform.position - transform.position  ;

            // Normalise so the force does not depend on the distance
            pushDirection.Normalize();

            playerController.ReceiveDamageFromForceField(pushDirection, dashForceFieldValue);
        }
    }
}

