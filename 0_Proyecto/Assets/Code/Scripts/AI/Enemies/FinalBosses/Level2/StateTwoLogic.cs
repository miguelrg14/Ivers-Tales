using RenownedGames.Apex;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateTwoLogic : MonoBehaviour
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
    public float pauseDuration = 3f;
    [Group("Atributtes")]
    [SerializeField]
    public float dashForceFieldValue = 40;
    [Group("Atributtes Fan")]
    [SerializeField]
    float initialAngle = -45f;
    [Group("Atributtes Fan")]
    [SerializeField]
    float finalAngle = 45f;
    public bool resting { get; private set; } = false;
    public bool shoot { get; private set; } = false;

    [Group("References")]
    [SerializeField]
    private BulletObjectPooling bulletObjectPooling;
    [Group("References")]
    [SerializeField]
    private PlayerDetectionSensor forceField;

    [Group("References")]
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
    void OnTriggerStay(Collider other)
    {
        // Verifica si el objeto que entra en contacto tiene un Rigidbody
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Calcula la dirección opuesta al vector de la posición del objeto hacia la posición del campo de fuerza
            Vector3 direccionEmpuje = transform.position - other.transform.position;

            // Normaliza la dirección para que tenga una magnitud de 1
            direccionEmpuje.Normalize();

            // Aplica la fuerza opuesta al objeto
            rb.AddForce(direccionEmpuje * 10, ForceMode.Force);
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
        Player_controler player_Controler = player.GetComponent<Player_controler>();
        if (player_Controler != null)
        {
            // Calcula la dirección opuesta al vector de la posición del objeto hacia la posición del campo de fuerza
            Vector3 direccionEmpuje = player.transform.position - transform.position  ;

            // Normaliza la dirección para que tenga una magnitud de 1
            direccionEmpuje.Normalize();

            player_Controler.RecieveDamageFromForceField(direccionEmpuje, dashForceFieldValue);
        }
    }
}

