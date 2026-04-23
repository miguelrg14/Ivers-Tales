using RenownedGames.Apex;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    //[Group("References")]
    //[SerializeField]
    private VFXManager vfxManager;
    // Start is called before the first frame update
    void Awake()
    {
        vfxManager=FindObjectOfType<VFXManager>();
    }

    // Update is called once per framed
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "BulletMortar")
        {
            AudioManager.instance.PlayClip(SoundsFX.SFX_FireContact);

            vfxManager.ActivateVFXBullet(other.gameObject.transform.position, Quaternion.identity);
            other.GetComponent<BulletLongDistanceEnemy>().age = 0;
            other.GetComponent<BulletLongDistanceEnemy>().SplashDamage(other);
        }
        else if (other.tag == "Bullet")
        {
            AudioManager.instance.PlayClip(SoundsFX.SFX_FireContact);
            Quaternion rotation = Quaternion.LookRotation(other.transform.position - transform.position, Vector3.forward);
            other.gameObject.SetActive(false);
            vfxManager.ActivateVFXBullet(other.gameObject.transform.position, rotation);

        }
        else if (other.tag == "BulletTornado")
        {
            AudioManager.instance.PlayClip(SoundsFX.SFX_FireContact);
            Quaternion rotationInverse = Quaternion.Inverse(other.transform.rotation);

            other.gameObject.SetActive(false);
            vfxManager.ActivateVFXBullet(other.gameObject.transform.position, rotationInverse);

        }
    }
}
