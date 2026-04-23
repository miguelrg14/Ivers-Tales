using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ControlBindingOptionManager : MonoBehaviour
{
    PlayerInput playerInput;
    [SerializeField] InputActionReference actionToRemap;
    InputActionRebindingExtensions.RebindingOperation rebindingOperation;

    [SerializeField] Slider sensivitySlider;  // Sensibilidad (solo en el menú, tmb toco esto ingame en el movimiento d la cám ("FPS")

    // Input game control Buttons
    [SerializeField] Button menu_Button;
    [SerializeField] Button moveForward_Button;
    [SerializeField] Button moveLeft_Button;
    [SerializeField] Button moveBack_Button;
    [SerializeField] Button moveRight_Button;
    [SerializeField] Button dash_Button;
    [SerializeField] Button attack_Button;
    [SerializeField] Button interact_Button;
    [SerializeField] Button crouch_Button;
    [SerializeField] Button seeStats_Button;
    TextMeshProUGUI sensivitySlider_Button_Text;
    TextMeshProUGUI menu_Button_Text;
    TextMeshProUGUI moveForward_Button_Text;
    TextMeshProUGUI moveLeft_Button_Text;
    TextMeshProUGUI moveBack_Button_Text;
    TextMeshProUGUI moveRight_Button_Text;
    TextMeshProUGUI dash_Button_Text;
    TextMeshProUGUI attack_Button_Text;
    TextMeshProUGUI interact_Button_Text;
    TextMeshProUGUI crouch_Button_Text;
    TextMeshProUGUI seeStats_Button_Text;

    float sensivity_default = 2;

    void Awake()
    {
        //GetReferences();
    }
    void Start()
    {
        //InitializeVariables();
    }

    public void SetSensivity(float value)
    {
        value = sensivitySlider.value;
        PlayerPrefs.SetFloat("sensivity", value);
        sensivitySlider_Button_Text.text = PlayerPrefs.GetFloat("sensivity").ToString("F2");
    }

    void GetReferences()
    {
        sensivitySlider_Button_Text = sensivitySlider.GetComponentInChildren<TextMeshProUGUI>();
        menu_Button_Text = menu_Button.GetComponentInChildren<TextMeshProUGUI>();
        moveForward_Button_Text = moveForward_Button.GetComponentInChildren<TextMeshProUGUI>();
        moveLeft_Button_Text = moveLeft_Button.GetComponentInChildren<TextMeshProUGUI>();
        moveBack_Button_Text = moveBack_Button.GetComponentInChildren<TextMeshProUGUI>();
        moveRight_Button_Text = moveRight_Button.GetComponentInChildren<TextMeshProUGUI>();
        dash_Button_Text = dash_Button.GetComponentInChildren<TextMeshProUGUI>();
        attack_Button_Text = attack_Button.GetComponentInChildren<TextMeshProUGUI>();
        interact_Button_Text = interact_Button.GetComponentInChildren<TextMeshProUGUI>();
        crouch_Button_Text = crouch_Button.GetComponentInChildren<TextMeshProUGUI>();
        seeStats_Button_Text = seeStats_Button.GetComponentInChildren<TextMeshProUGUI>();
    }
    void InitializeVariables()
    {
        playerInput = new PlayerInput(); // Initialize Player input to use new input system
        playerInput.Enable(); // Enable input system!

        InitializeBindingButtons_OptionsMenu();
        LoadControlsSettings();
        InitializeSliderPercentajes_Text();
    }
    void InitializeBindingButtons_OptionsMenu()
    {
        menu_Button_Text.text = playerInput.Player.Menu.GetBindingDisplayString();
        moveForward_Button_Text.text = playerInput.Player.Move.GetBindingDisplayString();
        //moveLeft_Button_Text.text = playerInput.Player.Movement.GetBindingDisplayString();
        //moveBack_Button_Text.text = playerInput.Player.Movement.GetBindingDisplayString();
        //moveRight_Button_Text.text = playerInput.Player.Movement.GetBindingDisplayString();
        dash_Button_Text.text = playerInput.Player.Dash.GetBindingDisplayString();
        attack_Button_Text.text = playerInput.Player.Fire.GetBindingDisplayString();
        interact_Button_Text.text = playerInput.Player.Interact.GetBindingDisplayString();
        crouch_Button_Text.text = playerInput.Player.Crouch.GetBindingDisplayString();
        seeStats_Button_Text.text = playerInput.Player.SeeInfo.GetBindingDisplayString();
    }

    void LoadControlsSettings()         // Paso los valores de las opciones al "PlayerPrefs" para q no se pierdan con el cambio de escena
    {
        if (PlayerPrefs.HasKey("sensivity"))
            sensivitySlider.value = PlayerPrefs.GetFloat("sensivity");
        else
        {
            sensivitySlider.value = sensivity_default;
            PlayerPrefs.SetFloat("sensivity", sensivity_default);
        }
    }

    void InitializeSliderPercentajes_Text()
    {
        sensivitySlider_Button_Text.text = PlayerPrefs.GetFloat("sensivity").ToString("F2");
    }

    public void Reset_ControlOptions()
    {
        sensivitySlider.value = sensivity_default;
        PlayerPrefs.SetFloat("sensivity", sensivity_default);
    }
}
