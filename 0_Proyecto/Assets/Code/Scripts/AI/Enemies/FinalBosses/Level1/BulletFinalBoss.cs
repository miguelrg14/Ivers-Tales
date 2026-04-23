using RenownedGames.Apex;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BulletFinalBoss : MonoBehaviour
{
    [SerializeField]
    private float AutoDestroyTime = 5f;

    [SerializeField] private float Force = 50;

    [SerializeField]
    private float bulletDamage = 10;

    [SerializeField]
    private ParticleSystem particleSystemBullet;
    private WaitForSeconds Wait;
    private Rigidbody Rigidbody;

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
    }
    private void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(DelayDisable());
    }

    private IEnumerator DelayDisable()
    {
        if (Wait == null)
        {
            Wait = new WaitForSeconds(AutoDestroyTime);
        }

        yield return null;

        Rigidbody.AddForce(transform.forward * Force);

        yield return Wait;
        
        gameObject.SetActive(false);
    }
    public void ResetValues()
    {
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
        return (int)bulletDamage;
    }
}
