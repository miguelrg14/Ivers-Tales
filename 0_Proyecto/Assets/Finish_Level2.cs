using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Finish_Level2 : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Ajusta la etiqueta según tus necesidades
        {
            StartCoroutine(WaitAndInvokeEvent());
        }
    }

    IEnumerator WaitAndInvokeEvent()
    {
        yield return new WaitForSeconds(1); // Espera 5 segundos
        ChangeToCreditsScene();

    }

    // Método para cambiar a la escena "credits"
    public void ChangeToCreditsScene()
    {
        SceneManager.LoadScene("Credits");
    }
}
