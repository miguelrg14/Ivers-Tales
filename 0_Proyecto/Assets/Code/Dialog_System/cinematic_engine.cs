using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class cinematic_engine : MonoBehaviour
{

    [SerializeField]
    SO_cinematics[] escenas;
    [SerializeField]
    private Image background;
    [SerializeField]
    int index;
    [SerializeField]
    int index_text;
    [SerializeField]
    private Image char_sprite;
    [SerializeField]
    private TMP_Text char_name;
    [SerializeField]
    private TMP_Text dialog;
    [SerializeField]
    private Image text_sprite;
    [SerializeField]
    private GameObject player_controler;
    //public float requiredHoldTime = 2.0f;
    //[SerializeField]
    //private float holdTime = 0.0f;
    [SerializeField]
    UnityEvent evento;
    [SerializeField]
    float time_scale_init;
    void Start()
    {
        index= 0;
        index_text=0;
        Time.timeScale = time_scale_init;
        escene_change();


    }
    private void OnDestroy()
    {
        evento.Invoke();
        Time.timeScale = 1f;
        if (player_controler)
        {
            player_controler.SetActive(true);
        }
        
        
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Return) || Input.GetButtonDown("B"))                 
        {


                Debug.Log("Omitiendo escena");
                omit_escene();

            

        }
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("A"))
        {
            escene_change();
        }

    }

    public void omit_escene() 
    
    {
        evento.Invoke();
        Destroy(this.gameObject);

    }

    public void escene_change()
    {
        if (index == escenas.Length)
        {

            Destroy(this.gameObject);
        }
        else
        {

            if (index_text < escenas[index].dialog.Length)
            {
               
                Debug.Log("cvarios dialogos");
                if (escenas[index].transp == true)
                {
                    background.color = new Color(0, 0, 0, 0);
                }
                background.sprite = escenas[index].background;
                char_name.text = escenas[index].character_name;
                if (escenas[index].char_image)
                {
                    char_sprite.sprite = escenas[index].char_image;
                }

                text_sprite.sprite = escenas[index].text_box;
                dialog.text = escenas[index].dialog[index_text];
                index_text++;

            }
            else 
            {
                if (index+1 != escenas.Length )
                {
                    index++;
                    index_text = 0;
                    Debug.Log("cambio de escena");
                    if (escenas[index].transp == true)
                    {
                        background.color = new Color(0, 0, 0, 0);
                    }
                    background.sprite = escenas[index].background;

                    char_name.text = escenas[index].character_name;
                    char_sprite.sprite = escenas[index].char_image;
                    text_sprite.sprite = escenas[index].text_box;
                    dialog.text = escenas[index].dialog[index_text];
                    index_text++;
                }
                else
                {
                    Destroy(this.gameObject);
                }

            
            }

        }
        if (index == escenas.Length)
        {
            Destroy(this.gameObject);
        }

    }

    
}
