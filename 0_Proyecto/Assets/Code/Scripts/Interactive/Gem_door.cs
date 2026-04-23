using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem_door : MonoBehaviour
{
    public Door_switch puerta1;
    public Door_switch puerta2;
    private GameObject lightGO;
    private GameObject gemGO;
    private GameObject gemGODisappear;
    private ParticleSystem particleSystemVFX;
    bool enableDisappear = false;

    private void Awake()
    {
        lightGO = transform.GetChild(0).gameObject;
        gemGO = transform.GetChild(1).gameObject;
        gemGODisappear = transform.GetChild(2).gameObject;
    }
    private void OnTriggerEnter(Collider other)
    {
        AudioManager.instance.PlayClip(SoundsFX.SFX_Gem_Pickup);
        puerta1.Off();
        if (puerta2 != null)
        {
            puerta2.Off();
        }
        gemGO.SetActive(false);
        lightGO.SetActive(false);
        gemGODisappear.SetActive(true);
        enableDisappear = true;
        particleSystemVFX = gemGODisappear.GetComponent<ParticleSystem>();

        //Destroy(this.gameObject);
    }

    void Start()
    {
        particleSystemVFX = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (enableDisappear &&!particleSystemVFX.IsAlive())
        {
            //particleSystemVFX.Stop();
            gameObject.SetActive(false);
            // O puedes desactivar directamente el sistema de partículas sin desactivar todo el objeto:
            // particleSystem.gameObject.SetActive(false);
        }
    }
}
