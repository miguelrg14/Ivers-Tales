using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableCryptTransition : MonoBehaviour
{
    [SerializeField]
    private Animator cryptAnimator;
    private void OnTriggerEnter(Collider other)
    {
        cryptAnimator.SetTrigger("open");
        cryptAnimator.gameObject.GetComponent<BoxCollider>().enabled = false;
    }
}
