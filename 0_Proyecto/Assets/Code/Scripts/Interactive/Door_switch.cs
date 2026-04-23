using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door_switch : MonoBehaviour
{
    public GameObject door1;
    public GameObject door2;
    public GameObject gem;

    private void Start()
    {
        On();
    }
    public void On()
    {
        door1.SetActive(true);
        door2.SetActive(true);
        gem.SetActive(false);
    }
    public void Off()
    {
        door1.SetActive(false);
        door2.SetActive(false);
        gem.SetActive(true);
    }
}
