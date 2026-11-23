using UnityEngine;

/// <summary>Opens the crypt (plays its "open" animation) when the player walks into the trigger.</summary>
public class EnableCryptTransition : MonoBehaviour
{
    [SerializeField] Animator cryptAnimator;

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<PlayerController>() == null) return;
        cryptAnimator.SetTrigger("open");
        var blocker = cryptAnimator.GetComponent<BoxCollider>();
        if (blocker != null) blocker.enabled = false;
    }
}
