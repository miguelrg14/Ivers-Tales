/* ================================================================
   ----------------------------------------------------------------
   Project   :   Iver Tales
   Publisher :   IguanaGo Studios
   Developer :   Lucas García Domínguez
   ----------------------------------------------------------------
   Copyright 2023-2024 IguanaGoStudios All rights reserved.
*/

using UnityEngine;
[RequireComponent(typeof(SphereCollider))]
[DefaultExecutionOrder(1)]
public class PlayerDetectionSensor : MonoBehaviour
{
    public delegate void PlayerEnterEvent(Transform Player);    //their firms
    public delegate void PlayerExitEvent(Vector3 LastKnownPosition);

    public event PlayerEnterEvent OnPlayerEnter;    //events for when the player enter and exits
    public event PlayerExitEvent OnPlayerExit;
    private void OnTriggerEnter(Collider other)
    {
        PlayerController player=other.GetComponentInParent<PlayerController>();
        if (player != null )
            OnPlayerEnter?.Invoke(player.transform);    //sets when the player enters
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerController player = other.GetComponentInParent<PlayerController>();
        if (player != null)
            OnPlayerExit?.Invoke(other.transform.position); //same for exits
    }
}
