using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchBossL2 : MonoBehaviour
{
    //[Header("References")]
    //[SerializeField]
    private ParticleSystem particleSystemTorch;
    public bool torchEnabled { get; private set; }

    void Awake()
    {
        particleSystemTorch = GetComponentInChildren<ParticleSystem>();
    }

    void Update()
    {
        //particleSystemTorch.Play();
    }
    public void Activate()
    {
        particleSystemTorch.Play();
        torchEnabled = true;

    }
}
