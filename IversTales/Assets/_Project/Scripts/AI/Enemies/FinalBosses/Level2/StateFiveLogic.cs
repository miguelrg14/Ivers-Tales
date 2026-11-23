using UnityEngine;

public class StateFiveLogic : MonoBehaviour
{
    [Header("Atributtes")]
    [SerializeField]
    public bool isEnabledThisState = true;
    public bool startBattle = false;
    [SerializeField]
    public Animator wallFinal;
    private VFXManager vfxManager;
    private void Awake()
    {
        vfxManager = FindObjectOfType<VFXManager>();
        //rightHand = transform.GetChild(0).GetChild(0).gameObject;
        //leftHand = transform.GetChild(0).GetChild(1).gameObject;

    }
    public void StartPhase5()
    {
        wallFinal.SetTrigger("EnableFinal");

        //obstaclesAnimator.SetTrigger("EnableState3");
        //torchesAnimator.SetTrigger("EnableState3");
        //Debug.Log("Phase4");
        //StartCoroutine(AttacksPhase4());
        //initialPosition = transform.position;
        //SetVFXWhenWallsAreDown();
    }

}
