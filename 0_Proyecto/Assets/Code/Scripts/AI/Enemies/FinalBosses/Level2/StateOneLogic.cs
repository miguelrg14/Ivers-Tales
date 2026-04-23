using RenownedGames.Apex;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateOneLogic : MonoBehaviour
{
    [Group("Atributtes")]
    [SerializeField]
    public bool isEnabledThisState = true;
    public bool startBattle = false;
    [Group("Atributtes")]
    [SerializeField]
    float projectileSpeed = 5f;
    [Group("Atributtes")]
    [SerializeField]
    float shootInterval = 1f;
    [Group("Atributtes")]
    [SerializeField]
    int shotsBeforePause = 3;
    [Group("Atributtes")]
    [SerializeField]
    float pauseDuration = 3f;
    [Group("Atributtes Fan")]
    [SerializeField]
    float initialAngle = -45f;
    [Group("Atributtes Fan")]
    [SerializeField]
    float finalAngle = 45f;
    public bool resting { get;private  set; } =false;
    public bool shoot { get; private set; } = false;

    [Group("References")]
    [SerializeField]
    private BulletObjectPooling bulletObjectPooling;

    public void StartShooting()
    {
        StartCoroutine(ShootInIntervals());
    }

    void ShootFan(int shotsCount)
    {
        float value = 10f;

        if (shotsCount % 2 == 0)
        {
            // Ronda par: Disparar bolas en lugares diferentes
            for (float angle = initialAngle; angle <= finalAngle; angle += value)
            {
                Vector3 direction = Quaternion.Euler(0f, angle, 0f) * transform.forward;
                FireProjectile(direction);
            }
        }
        else
        {
            // Ronda impar: Disparar bolas en lugares diferentes
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
    IEnumerator ShootInIntervals()
    {
        int shotsCount = 0;
        yield return new WaitForSeconds(2f);    //for starting the battle
        shoot = true;

        while (isEnabledThisState)
        {

            if (shotsCount==shotsBeforePause)
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
}
