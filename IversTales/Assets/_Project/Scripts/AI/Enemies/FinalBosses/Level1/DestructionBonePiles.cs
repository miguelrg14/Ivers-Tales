using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructionBonePiles : MonoBehaviour
{
    [SerializeField]
    GameObject destructionPileBones;
    private void OnTriggerEnter(Collider other)
    {
        this.gameObject.SetActive(false);
        destructionPileBones.SetActive(true);
    }
}
