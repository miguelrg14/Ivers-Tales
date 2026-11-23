using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// Controls tab of the options menu. The bindings are fixed and only shown as
/// images, so the only editable value is the sensitivity, saved in PlayerPrefs.
/// </summary>
public class ControlBindingOptionManager : MonoBehaviour
{
    public const string SensitivityKey = "sensivity"; // original key, kept for existing saves
    public const float DefaultSensitivity = 2f;

    [SerializeField, FormerlySerializedAs("sensivitySlider")] Slider sensitivitySlider;

    /// <summary>Saved sensitivity, available to camera or aiming scripts.</summary>
    public static float Sensitivity => PlayerPrefs.GetFloat(SensitivityKey, DefaultSensitivity);

    void Awake()
    {
        if (sensitivitySlider != null)
            sensitivitySlider.onValueChanged.AddListener(SetSensitivity);
    }

    void Start() => Refresh(Sensitivity);

    public void SetSensitivity(float value)
    {
        PlayerPrefs.SetFloat(SensitivityKey, value);
        UpdateLabel(value);
    }

    public void ResetControlOptions()
    {
        PlayerPrefs.SetFloat(SensitivityKey, DefaultSensitivity);
        Refresh(DefaultSensitivity);
    }

    void Refresh(float value)
    {
        if (sensitivitySlider != null) sensitivitySlider.SetValueWithoutNotify(value);
        UpdateLabel(value);
    }

    void UpdateLabel(float value)
    {
        if (sensitivitySlider == null) return;
        var label = sensitivitySlider.GetComponentInChildren<TextMeshProUGUI>();
        if (label != null) label.text = value.ToString("F2");
    }
}
