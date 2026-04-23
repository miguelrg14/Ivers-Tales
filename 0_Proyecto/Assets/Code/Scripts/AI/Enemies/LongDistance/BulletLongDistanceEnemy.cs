/* ================================================================
   ----------------------------------------------------------------
   Project   :   Iver Tales
   Publisher :   IguanaGo Studios
   Developer :   Lucas García Domínguez
   ----------------------------------------------------------------
   Copyright 2023-2024 IguanaGoStudios All rights reserved.
*/

using RenownedGames.Apex;
using System.Collections;
using System.Drawing;
using UnityEngine;
using Color = UnityEngine.Color;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class BulletLongDistanceEnemy : MonoBehaviour
{
    [SerializeField]
    GameObject dianaPrefab; // Prefab de la diana
    GameObject dianaInstance;
    [SerializeField]
    private float autoDestroyTime = 5f;

    [SerializeField] private float force = 100;
    [SerializeField]
    private float bulletDamage = 10;

    [SerializeField]
    private ParticleSystem particleSystemBullet;
    private WaitForSeconds Wait;
    private Rigidbody Rigidbody;
    public float age { private get; set; }
    Vector3 launchPoint, targetPoint, launchVelocity;
    float groundHeight = 1.0f; // Altura del suelo (puedes obtenerla de otro lugar si es dinámica)

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        groundHeight += 0.3f;
    }
    public void SetStartValues(Vector3 launchPoint, Vector3 targetPoint, Vector3 launchVelocity,float groundHeight)
    {
        this.groundHeight=groundHeight;
        this.launchPoint = launchPoint;
        this.targetPoint = targetPoint;
        this.launchVelocity = launchVelocity;
        transform.position = launchPoint;
        CalculateTrajectory();
    }
    private void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(DelayDisable());
    }
    private Vector3 CalculateImpactPoint()
    {
        float a = 0.5f * Physics.gravity.y;
        float b = launchVelocity.y;
        float c = launchPoint.y - groundHeight;

        // Calcula el discriminante
        float discriminant = b * b - 4 * a * c;

        // Calcula el tiempo en el que la altura alcanza el valor deseado
        float timeToReachDesiredHeight;
        if (discriminant >= 0)
        {
            float t1 = (-b + Mathf.Sqrt(discriminant)) / (2 * a);
            float t2 = (-b - Mathf.Sqrt(discriminant)) / (2 * a);
            timeToReachDesiredHeight = Mathf.Max(t1, t2);
        }
        else
        {
            // Si el discriminante es negativo, establecemos el tiempo en cero (o algún otro valor adecuado)
            timeToReachDesiredHeight = 0f;
        }

        // Calcula la posición en ese momento
        Vector3 impactPoint = launchPoint + launchVelocity * timeToReachDesiredHeight;
        impactPoint.y = groundHeight;

        return impactPoint;
    }
    private void CalculateTrajectory()
    {
        // Calcular el punto de impacto previsto en el suelo
        Vector3 impactPoint = CalculateImpactPoint();
        // Coloca la diana en el punto de impacto previsto
        dianaInstance = Instantiate(dianaPrefab, impactPoint,dianaPrefab.transform.rotation);
        dianaInstance.transform.position = 
            new Vector3(impactPoint.x, groundHeight, impactPoint.z);
    }
    private IEnumerator DelayDisable()
    {
        if (Wait == null)
        {
            Wait = new WaitForSeconds(autoDestroyTime);
        }

        yield return null;

        Rigidbody.AddForce(transform.forward * force);

        yield return Wait;
        ResetValues();
        DesactivateDiana();
        gameObject.SetActive(false);
    }
    public void ResetValues()
    {
        this.age = 0;
        this.particleSystemBullet.Stop();
        gameObject.SetActive(false);
    }
    private void OnDisable()
    {
        Rigidbody.angularVelocity = Vector3.zero;
        Rigidbody.velocity = Vector3.zero;
    }
    public int GetBulletDamage()
    {
        AudioManager.instance.PlayClip(SoundsFX.SFX_FireContact);
        return (int)bulletDamage;
    }

    public void Update()
    {
        Vector3 d = launchVelocity;
        d.y -= 9.81f * age;
        transform.localRotation = Quaternion.LookRotation(d);
        age += Time.deltaTime;
        Vector3 p = launchPoint + launchVelocity * age;
        p.y -= 0.5f * 9.81f * age * age;
        transform.position = p;
    }
    public void SplashDamage(Collider other)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 4f);
        //this.GetComponent<CapsuleCollider>().radius = 8f;
        foreach (Collider item in colliders)
        {
            if (item.tag == "Player")
            {
                Debug.Log("Coming");
                Player_controler player_Controler = item.GetComponentInParent<Player_controler>();

                player_Controler.RecieveDamageFromBullets(other);
            }
        }
        DesactivateDiana();
        this.gameObject.SetActive(false);

    }
    public void DesactivateDiana()
    {
        dianaInstance.SetActive(false);

    }
    private void OnDrawGizmos()
    {
        // Dibujar la esfera de debug en el editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 4f);
    }
}
