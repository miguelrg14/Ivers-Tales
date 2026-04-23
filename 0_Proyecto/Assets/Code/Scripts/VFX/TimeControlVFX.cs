using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeControlVFX : MonoBehaviour
{
    private ParticleSystem particleSystemVFX;

    void Start()
    {
        particleSystemVFX = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (!particleSystemVFX.IsAlive())
        {
            //particleSystemVFX.Stop();
            gameObject.SetActive(false);
            // O puedes desactivar directamente el sistema de partículas sin desactivar todo el objeto:
            // particleSystem.gameObject.SetActive(false);
        }
    }
}
