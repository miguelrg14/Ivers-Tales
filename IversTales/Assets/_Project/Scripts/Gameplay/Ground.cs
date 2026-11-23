using UnityEngine;

/// <summary>
/// Floor trigger that reacts to enemy projectiles: mortar shells explode with
/// splash damage, boss and tornado bullets are returned to their pool.
/// </summary>
public class Ground : MonoBehaviour
{
    VFXManager vfxManager;

    void Awake()
    {
        vfxManager = FindObjectOfType<VFXManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BulletMortar"))
        {
            var shell = other.GetComponent<BulletLongDistanceEnemy>();
            if (shell == null) return;
            AudioManager.Instance?.PlayClip(SoundsFX.SFX_FireContact);
            vfxManager.ActivateVFXBullet(other.transform.position, Quaternion.identity);
            shell.age = 0;
            shell.SplashDamage(other);
        }
        else if (other.CompareTag("Bullet"))
        {
            AudioManager.Instance?.PlayClip(SoundsFX.SFX_FireContact);
            Quaternion rotation = Quaternion.LookRotation(other.transform.position - transform.position, Vector3.forward);
            other.gameObject.SetActive(false);
            vfxManager.ActivateVFXBullet(other.transform.position, rotation);
        }
        else if (other.CompareTag("BulletTornado"))
        {
            AudioManager.Instance?.PlayClip(SoundsFX.SFX_FireContact);
            other.gameObject.SetActive(false);
            vfxManager.ActivateVFXBullet(other.transform.position, Quaternion.Inverse(other.transform.rotation));
        }
    }
}
