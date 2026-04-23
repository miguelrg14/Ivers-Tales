using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "Cinematic_data", menuName = "ScriptableObjects/cinematic_data", order = 1)]
public class SO_cinematics : ScriptableObject
{
    public string character_name;
    public Sprite text_box;
    public Sprite background;
    public Sprite char_image;
    public string[] dialog;
    public bool transp=false;



}
