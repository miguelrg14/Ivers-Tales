using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Combat : MonoBehaviour
{
    public List<Attack_SO> combo;
    float lastclickedTime;
    float LastComboEnd;
    int comboCounter;

    public Animator anim;
    [SerializeField] Weapon weapon;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Attack"))
        {
            attack();
        }
        ExitAttack();
    }

    void attack()
    {
        anim.SetTrigger("attack");
        weapon.EnabledTriggerBox();
        //if (Time.time-LastComboEnd>0.5f && comboCounter<=combo.Count)
        //{
        //    CancelInvoke("EndCombo");
        //    if (Time.time-lastclickedTime>=0.2f)
        //    {
        //        anim.runtimeAnimatorController = combo[comboCounter].animatorDV;
        //        anim.Play("Attack", 0, 0);
        //        weapon.damage = combo[comboCounter].damage;
        //        comboCounter++;
        //        lastclickedTime = Time.time;

        //        if (comboCounter > combo.Count)
        //        {
        //            comboCounter = 0;
        //        }
        //    }

        //}
    }
    void ExitAttack()
    {
        //if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime>0.9f && anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
        //{
        //    Invoke("EndCombo",1);
        //}
    }

    void EndCombo()
    {
        comboCounter = 0;
        LastComboEnd = Time.time;
    }

}
