using UnityEngine;

/// <summary>Pair of doors that open (disappear) when their gem is collected.</summary>
public class DoorSwitch : MonoBehaviour
{
    public GameObject door1;
    public GameObject door2;
    [Tooltip("Gem shown in the door frame once the door is open.")]
    public GameObject gem;

    void Start() => On();

    /// <summary>Doors closed.</summary>
    public void On() => SetOpen(false);

    /// <summary>Doors open.</summary>
    public void Off() => SetOpen(true);

    void SetOpen(bool open)
    {
        if (door1 != null) door1.SetActive(!open);
        if (door2 != null) door2.SetActive(!open);
        if (gem != null) gem.SetActive(open);
    }
}
