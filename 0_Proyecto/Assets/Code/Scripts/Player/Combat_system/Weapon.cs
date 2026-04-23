using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float damage;
    BoxCollider triggerBox;
    void Start()
    {
         triggerBox=GetComponent<BoxCollider>();
        DisabledTriggerBox();
    }

    private void OnTriggerEnter(Collider other)
    {
        var enemy =other.gameObject;
        if (enemy != null)
        {     /*  Falta implementar el enemigo y su interfaz*/
            //enemy.healt.hp -= damage;
            //if (enemy.health.hp <= 0)
            //{
            //    Destroy(enemy.gameObject);
            //}
        }
    }
    public void EnabledTriggerBox()
    {
        triggerBox.enabled = true;
    }
    public void DisabledTriggerBox()
    {
        //triggerBox.enabled = false;
    }
}
