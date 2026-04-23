using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShineUIEffect : MonoBehaviour
{
    public float shineDuration = 5.0f; // Duración del desplazamiento del brillo de 1 a 0
    public float pauseTime = 3.0f; // Tiempo de pausa cuando el brillo alcanza 0

    private float currentTime = 0f;
    private Image rend;
    private Material mat;

    void Start()
    {
        rend = GetComponent<Image>();
        mat = rend.material;

        StartCoroutine(UpdateShineLocation());
    }

    // Corrutina para actualizar el valor de _ShineLocation
    IEnumerator UpdateShineLocation()
    {
        while (true)
        {
            float timer = 0f;

            // Desplazamiento del brillo de 1 a 0
            while (timer < shineDuration)
            {
                float newShineLocation = Mathf.Lerp(1f, 0f, timer / shineDuration);
                mat.SetFloat("_ShineLocation", newShineLocation);
                timer += Time.deltaTime;
                yield return null;
            }

            // Pausa cuando el brillo alcanza 0
            yield return new WaitForSeconds(pauseTime);
        }
    }
}
