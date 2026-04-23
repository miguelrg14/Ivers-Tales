using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class preuba_cinema : MonoBehaviour
{
    public GameObject cinema;
    public GameObject player;
    [SerializeField]
    UnityEvent evento;
    private void Start()
    {
        if (    evento  == null)
        evento = new UnityEvent();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ( collision.CompareTag("Player"))
        {
            cinema.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            evento.Invoke();
            Destroy(this);
        }
    }
}
