using RenownedGames.AITree;
using RenownedGames.Apex;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class FinalBossLevelOne : MonoBehaviour
{
    [Group("Atributtes")]
    [SerializeField]
    public bool isEnabledBoss = true;
    public bool startBattle = false;
    [Group("Atributtes")]
    [SerializeField]
    float projectileSpeed = 5f;
    [Group("Atributtes")]
    [SerializeField]
    float shootInterval = 1f;
    [Group("Atributtes")]
    [SerializeField]
    int shotsBeforePause = 6;
    [Group("Atributtes")]
    [SerializeField]
    float pauseDuration = 3f;
    [Group("Atributtes Fan")]
    [SerializeField]
    float initialAngle = -45f;
    [Group("Atributtes Fan")]
    [SerializeField]
    float finalAngle = 45f;
    [Group("Health")]
    [SerializeField]
    float health = 100;

    [Group("References")]
    [SerializeField] 
    private Player_controler player;
    private VFXManager vfxManager;
    [Group("References")]
    [SerializeField] 
    private BulletObjectPooling bulletObjectPooling;
    [Group("References")]
    [SerializeField]
    ParticleSystem chargingPS;
    [Group("References")]
    [SerializeField]
    ParticleSystem chargingFinishPS;
    RotationShootSpawnLocation rotationShootSpawnLocation;
    private SpawnEffect disolveDeadEffect;
    Transform fanTransform;
    Renderer meshRenderer;
    [SerializeField]
    Animator animator;
    public Animator boss_cam_anim;
    private Color[] colorsOrigin;

    public GameObject finish_col;

    [Group("Phase 2")]
    [SerializeField]
    public bool enabledPhase2 { get;private set; } =false;
    [Group("Phase 2")]
    [SerializeField]
    GameObject shield;
    [Group("Phase 2")]
    [SerializeField]
    Torch[] torches = new Torch[4];
    bool activeTransition=false;
    bool firstTimeTransition=false;

    [Group("Audio")]
    [SerializeField]
    private AudioSource audioSource;
    [Group("Audio")]
    [SerializeField]
    AudioClip attack_clip;
    [Group("Audio")]
    [SerializeField]
    AudioClip hurt_clip;
    [Group("Audio")]
    [SerializeField]
    AudioClip death_clip;

    void Start()
    {
        rotationShootSpawnLocation = GetComponent<RotationShootSpawnLocation>();
        disolveDeadEffect = GetComponentInChildren<SpawnEffect>();
        //animator = GetComponent<Animator>();
        vfxManager=FindObjectOfType<VFXManager>();
        fanTransform = GetComponent<Transform>();
        rotationShootSpawnLocation.enabledRotation = true;
        rotationShootSpawnLocation.SetPlayer(player.transform);
        meshRenderer = GetComponentInChildren<Renderer>();
        colorsOrigin=new Color[meshRenderer.materials.Length];
        shield.SetActive(enabledPhase2);
        foreach (var t in torches)
        {
            t.SetParent(this);
            t.gameObject.SetActive(false);
        }

        for (int i = 0; i < meshRenderer.materials.Length; i++)
        {
            colorsOrigin[i] = meshRenderer.materials[i].color;
        }
        if (startBattle)
            StartCoroutine(ShootInIntervals());

    }
    private void Update()
    {
        if (startBattle)
        {
            startBattle = false;
            isEnabledBoss = true;
            StartCoroutine(ShootInIntervals());
        }
    }
    void ShootFan(int shotsCount)
    {
        float value = 10f;

        if (shotsCount % 2 == 0)
        {
            // Ronda par: Disparar bolas en lugares diferentes
            for (float angle = initialAngle; angle <= finalAngle; angle += value)
            {
                Vector3 direction = Quaternion.Euler(0f, angle, 0f) * transform.forward;
                FireProjectile(direction);
            }
        }
        else
        {
            // Ronda impar: Disparar bolas en lugares diferentes
            for (float angle = initialAngle + value / 2; angle <= finalAngle - value / 2; angle += value)
            {
                Vector3 direction = Quaternion.Euler(0f, angle, 0f) * transform.forward;
                FireProjectile(direction);
            }
        }
    }

    void FireProjectile(Vector3 direction)
    {
        audioSource.PlayOneShot(attack_clip);
        GameObject projectile = bulletObjectPooling.GetPooledObject();
        projectile.transform.position = new Vector3(transform.position.x,transform.position.y+1,transform.position.z);
        projectile.transform.rotation = Quaternion.LookRotation(direction);
        projectile.SetActive(true);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.velocity = direction * projectileSpeed;
    }
    IEnumerator ShootInIntervals()
    {
        int shotsCount = 0;

        while (isEnabledBoss)
        {
            if (enabledPhase2)
            {
                activeTransition = true;
                for (int i = 0; i < torches.Length; i++)
                {
                    if (!torches[i].torchEnabled)
                    {
                        activeTransition = false;
                    }
                }
                if (activeTransition)
                    ActiveTransitionPhase2to1();

            }
            animator.SetTrigger("Attack");
            ShootFan(shotsCount);

            shotsCount++;
            if (!enabledPhase2)
            {
                if (shotsCount >= shotsBeforePause)
                {
                    rotationShootSpawnLocation.enabledRotation = false;
                    animator.SetBool("Rest", true);
                    chargingPS.Play();
                    // Pause shooting for the specified duration
                    yield return new WaitForSeconds(pauseDuration - 1.30f);
                    animator.SetBool("Rest", false);
                    chargingPS.Stop();
                    chargingFinishPS.Play();
                    yield return new WaitForSeconds(1f);
                    rotationShootSpawnLocation.enabledRotation = true;

                    // Reset the shots count after the pause
                    shotsCount = 0;
                }
            }


            yield return new WaitForSeconds(shootInterval);
        }
    }

    public void RecieveDamage(int damage)
    {


        if (enabledPhase2)
        {


        }
        else
        {
            audioSource.PlayOneShot(hurt_clip);
            health -= damage;

            if (health == 50)
            {
                enabledPhase2 = true;
                shield.SetActive(enabledPhase2);
                foreach (var t in torches)
                {
                    t.gameObject.SetActive(true);
                }

            }

            StartCoroutine(FlashRecieveDamage());
            if (health == 0)
            {
                animator.SetTrigger("Die");
                boss_cam_anim.SetBool("active", false);
                rotationShootSpawnLocation.enabled = false;
                audioSource.PlayOneShot(death_clip);
                isEnabledBoss = false;
                vfxManager.ActivateVFXEnemyDie(this.transform.position, Quaternion.identity);
                finish_col.SetActive(true);

                disolveDeadEffect.StartDissableDeadEffect(this.gameObject);
            }
        }

    }
    void ActiveTransitionPhase2to1()
    {
        enabledPhase2=false;
        shield.SetActive(enabledPhase2);

    }
    IEnumerator FlashRecieveDamage()
    {
        foreach (Material t in meshRenderer.materials)
        {
            t.color = Color.red;
        }
        yield return new WaitForSeconds(0.15f);
        for (int i = 0; i < meshRenderer.materials.Length; i++)
        {
             meshRenderer.materials[i].color=colorsOrigin[i] ;

        }

    }
}
