/* ================================================================
   ----------------------------------------------------------------
   Project   :   Iver Tales
   Publisher :   IguanaGo Studios
   Developer :   Lucas García Domínguez
   ----------------------------------------------------------------
   Copyright 2023-2024 IguanaGoStudios All rights reserved.
*/

using UnityEngine;

public class RotationShootSpawnLocation : MonoBehaviour
{

    Transform player;
    public Vector3 direction {  get; private set; }
    public bool enabledRotation { private get; set; } =false;
    [SerializeField]
    bool isEnabledRotationY=false;
    public void SetPlayer(Transform player)
    {
        this.player = player;
    }

    void Update()
    {
        if (enabledRotation) {
            if (isEnabledRotationY)
            {
                direction = (player.position - transform.position).normalized;  //follows the target
                transform.rotation = Quaternion.LookRotation(direction);
            }
            else
            {
                direction = (player.position - transform.position).normalized;  //follows the target
                Vector3 newDirection=new Vector3(direction.x, 0,direction.z); // Keep the rotation on the horizontal plane
                transform.rotation = Quaternion.LookRotation(newDirection); // Face the player
            }

        }
    }
}
