using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torch : MonoBehaviour
{
    //[Group("References")]
    //[SerializeField]
    private VFXManager vfxManager;
    private FinalBossLevelOne finalBossLevelOne;
    private ParticleSystem particleSystemTorch;
    public bool torchEnabled { get; private set; }
    // Start is called before the first frame update
    void Awake()
    {
        vfxManager = FindObjectOfType<VFXManager>();
        particleSystemTorch = GetComponentInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void SetParent(FinalBossLevelOne finalBoss)
    {
        finalBossLevelOne = finalBoss;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "BulletMortar")
        {
            AudioManager.instance.PlayClip(SoundsFX.SFX_FireContact);
            Quaternion rotationInverse = Quaternion.Inverse(other.transform.rotation);

            vfxManager.ActivateVFXBullet(other.gameObject.transform.position, rotationInverse);
            other.GetComponent<BulletLongDistanceEnemy>().age = 0;
            // other.gameObject.transform.localPosition = Vector3.zero;
            other.gameObject.SetActive(false);
        }
        else if (other.tag == "Bullet")
        {
            AudioManager.instance.PlayClip(SoundsFX.SFX_FireContact);
            other.gameObject.SetActive(false);
            vfxManager.ActivateVFXBullet(other.gameObject.transform.position,Quaternion.identity);

            if (finalBossLevelOne.enabledPhase2)
            {
                if (particleSystemTorch.isStopped)
                {
                    particleSystemTorch.Play();
                    torchEnabled = true;
                }
            }


        }
    }
}
