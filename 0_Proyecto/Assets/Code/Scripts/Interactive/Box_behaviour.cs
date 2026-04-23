using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box_behaviour : MonoBehaviour
{
    private bool moverse = true; // Variable para controlar si la caja debe moverse
    private Vector3 puntoFinal; // Punto final de la caja



    public void Move(Vector3 direccion, float distancia)
    {
        // Calcular el punto final basado en la dirección y la distancia proporcionadas
        puntoFinal = transform.position + direccion.normalized * distancia;

        // Mover instantáneamente la caja hacia el punto final
        transform.position = puntoFinal;

        // Si la caja ha llegado al punto final, detener el movimiento
        moverse = false;
    }
}

