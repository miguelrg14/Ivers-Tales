using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchBossL2 : MonoBehaviour
{
    //[Group("References")]
    //[SerializeField]
    private ParticleSystem particleSystemTorch;
    public bool torchEnabled { get; private set; }
    // Start is called before the first frame update
    void Awake()
    {
        particleSystemTorch = GetComponentInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        //particleSystemTorch.Play();
    }
    public void Activar()
    {
        particleSystemTorch.Play();
        torchEnabled = true;

        //rend.enabled = !rend.enabled; // Cambia el estado de visibilidad al opuesto del actual
    }
}
