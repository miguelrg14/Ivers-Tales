using RenownedGames.AITree.Demo;
using RenownedGames.Apex;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityHFSM;
using static UnityEngine.Rendering.DebugUI;
public class FinalBossLevelTwo : FinalBoss
{
    [Group("References")]
    [SerializeField]
    private Player_controler player;
    private active_boos active_Boos;
    private Animator animatorActive_Boos;
    //[Group("References")]
    //[SerializeField]
    //private BulletObjectPooling bulletObjectPooling;
    private StateOneLogic oneLogic;
    private StateTwoLogic twoLogic;
    private StateThreeLogic threeLogic;
    private StateFourLogic fourLogic;
    private StateFiveLogic fiveLogic;

    [Group("Health")]
    [SerializeField]
    float health = 100;
    RotationShootSpawnLocation rotationShootSpawnLocation;
    [Group("Atributtes")]
    [SerializeField]
    TorchBossL2[] torchesBossL2= new TorchBossL2[2];
    [Group("Atributtes")]
    [SerializeField]
    public bool isEnabledBoss = true;
    bool beginBattle=false;
    private StateMachine<BossState, BossStateEvent> bossHSM;
    SkinnedMeshRenderer[] meshRenderers=new SkinnedMeshRenderer[2];
    List<Material> materials=new List<Material>();

    Color[] colorsOrigin;

    //private Animator animator;
    private void Awake()
    {
        active_Boos=FindObjectOfType<active_boos>();
        animatorActive_Boos=active_Boos.gameObject.GetComponent<Animator>();
        meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        int account = 0;
        foreach (var item in meshRenderers)
        {
            foreach (var material in item.materials)
            {
                materials.Add(material);
                account++;
            }
        }
        colorsOrigin = new Color[account];
        for (int i = 0; i < colorsOrigin.Length; i++)
        {
            colorsOrigin[i] = materials [i].color;
        }
        //animator = GetComponentInChildren<Animator>();
        oneLogic = GetComponent<StateOneLogic>();
        twoLogic = GetComponent<StateTwoLogic>();
        threeLogic = GetComponent<StateThreeLogic>();   
        fourLogic = GetComponent<StateFourLogic>();
        fiveLogic = GetComponent<StateFiveLogic>();

        SetRotationFirstStatus();
        bossHSM = new StateMachine<BossState, BossStateEvent>();
        bossHSM.AddState(BossState.State1, new StateOne(true, player, this, oneLogic));
        bossHSM.AddState(BossState.State2, new StateTwo(true, player, this, twoLogic));
        bossHSM.AddState(BossState.State3, new StateThree(true, player, this, threeLogic));
        bossHSM.AddState(BossState.State4, new StateFour(true, player, this, fourLogic));
        bossHSM.AddState(BossState.State5, new StateFive(false, player, this, fiveLogic));

        bossHSM.AddTriggerTransition(BossStateEvent.TorchesEnabled,
            new Transition<BossState>(BossState.State1,BossState.State2));
        bossHSM.AddTriggerTransition(BossStateEvent.GetsShoot,
            new Transition<BossState>(BossState.State2, BossState.State3));

        bossHSM.AddTriggerTransition(BossStateEvent.Gets25PercentOfLife,
            new Transition<BossState>(BossState.State3, BossState.State4));

        bossHSM.AddTriggerTransition(BossStateEvent.Die,
    new Transition<BossState>(BossState.State4, BossState.State5));
        bossHSM.SetStartState(BossState.State1);

        if (isEnabledBoss)
        {
            bossHSM.SetStartState(BossState.State1);
            bossHSM.Init();
        }


    }
    void SetRotationFirstStatus()
    {
        rotationShootSpawnLocation = GetComponent<RotationShootSpawnLocation>();
        rotationShootSpawnLocation.SetPlayer(player.transform);
        rotationShootSpawnLocation.enabledRotation = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isEnabledBoss&&!beginBattle)
        {
            bossHSM.SetStartState(BossState.State1);
            bossHSM.Init();
            beginBattle = true;
        }
        if (isEnabledBoss)
        {
            bossHSM.OnLogic();
            int amount = 0;

            foreach (var item in torchesBossL2)
            {
                if (item.torchEnabled)
                {
                    amount++;
                }
            }

            if (amount == 2)
            {
                bossHSM.Trigger(BossStateEvent.TorchesEnabled);
            }
            if (health == 25)
            {

                if (bossHSM.ActiveStateName == BossState.State3)
                {
                    bossHSM.Trigger(BossStateEvent.Gets25PercentOfLife);
                }

            }
        }
    }
    public void RecieveDamage(int damage)
    {
        //audioSource.PlayOneShot(hurt_clip);
        health -= damage;

        if (health == 75)
        {
            twoLogic.pauseDuration = 2;
        }
        else if (health==50)
        {
            bossHSM.Trigger(BossStateEvent.GetsShoot);

        }


        StartCoroutine(FlashRecieveDamage());
        if (health == 0)
        {
            bossHSM.Trigger(BossStateEvent.Die);
            animatorActive_Boos.SetBool("active", false);

            //rotationShootSpawnLocation.enabled = false;
            //audioSource.PlayOneShot(death_clip);
            //isEnabledBoss = false;
            //vfxManager.ActivateVFXEnemyDie(this.transform.position, Quaternion.identity);
            //finish_col.SetActive(true);
            //disolveDeadEffect.StartDissableDeadEffect(this.gameObject);
        }


    }
    IEnumerator FlashRecieveDamage()
    {
        foreach (SkinnedMeshRenderer t in meshRenderers)
        {
            foreach (Material t2 in t.materials)
            {

                t2.color = Color.red;

            }
        }
        yield return new WaitForSeconds(0.15f);
        foreach (SkinnedMeshRenderer t in meshRenderers)
        {
            foreach (Material t2 in t.materials)
            {

                t2.color = Color.white;

            }
        }


    }
}
