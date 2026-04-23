using RenownedGames.Apex;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class skill_system : MonoBehaviour
{
    public LayerMask capaPlataformas;
    public float radioActivacion = 5f;
    
    public float index = 1;
    public GameObject fireballPrefab;
    [Group("References")]
    [SerializeField]
    ParticleSystem shootPS;
    [Group("References")]
    [SerializeField]
    ParticleSystem explosionActivatePlatformPS;
    [Group("References")]
    [SerializeField]
    ParticleSystem magicCircleActivatePlatformPS;
    public Transform firePoint;
    public bool fireEnabled=false;
    public Animator animator;
    public float fireballSpeed = 10f; // Velocidad de la bola de fuego
    public int fireballDamage = 10; // Daño de la bola de fuego
    public float cooldownDuration = 2f; // Duración del cooldown en segundos
    private bool isCooldown = false; // Variable para rastrear si la habilidad está en cooldown
    private float cooldownTimer = 0f; // Temporizador para controlar el cooldown
    public Transform wind_pos;
    public float wind_dist=2;
    [SerializeField]
    public bool isLevel2BossFight=false;

    [Group("Skill")]
    [SerializeField]
    private Image skillImage;

    [Group("Skill")]
    [SerializeField]
    private Sprite lightSkillEnabledSprite;

    [Group("Skill")]
    [SerializeField]
    private Sprite fireSkillEnabledSprite;

    [Group("Skill")]
    [SerializeField]
    private Sprite airSkillEnabledSprite;

    [Group("Skill")]
    [SerializeField]
    private Sprite skillDisabledSprite;

    [Group("Skill")]
    [SerializeField]
    private Sprite skillActivatedSprite;

    private Sprite skillActualSprite;

    [Group("Audio")]
    [SerializeField]
    private AudioSource audioSource;
    [Group("Audio")]
    [SerializeField]
    AudioClip hability_platform_clip;
    [Group("Audio")]
    [SerializeField]
    AudioClip hability_fire_clip;
    [Group("Audio")]
    [SerializeField]
    AudioClip hability_wind_clip;
    [SerializeField]
    Animator animatorCam;
    private void Update()
    {
        if (Input.GetButtonDown("Sk1"))
        {
            index = 1;
            skillImage.sprite = lightSkillEnabledSprite;
            StartCoroutine(DisableEffect());

        }
        if (Input.GetButtonDown("Sk2"))
        {
            index = 2;
            skillImage.sprite = fireSkillEnabledSprite;
            StartCoroutine(DisableEffect());
        }
        if (Input.GetButtonDown("Sk3"))
        {
            index = 3;
        }
        if (Input.GetButtonDown("Skill"))
        {
            switch (index)
            {
                
                case 1:
                    //skillActualSprite = lightSkillEnabledSprite;
                    skillImage.sprite = lightSkillEnabledSprite;
                    animator.SetTrigger("Skill1");
                    LightSkill();                    
                    StartCoroutine(DisableEffect());
                    audioSource.PlayOneShot(hability_platform_clip);

                    break;
                case 2:
                    if (!isCooldown&&fireEnabled == true)
                    {
                        //skillActualSprite=fireSkillEnabledSprite;
                        skillImage.sprite = fireSkillEnabledSprite;
                        audioSource.PlayOneShot(hability_fire_clip);

                        FireSkill();
                        //animatorCam.SetTrigger("EnableZoom");

                        StartCoroutine(DisableEffect());

                    }
                    break;
                case 3:
                    //TODO make the other sprites
                    audioSource.PlayOneShot(hability_wind_clip);

                    ActivateWindSkill(wind_pos.transform.position,2);
                    //animator.SetTrigger("Skill1");
                    break;
                case 0:
                    break;
            }
        }
        if (isCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0)
            {
                // Si el cooldown ha terminado, cambiamos el estado de cooldown
                isCooldown = false;
            }
        }

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radioActivacion);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(wind_pos.transform.position, 2);
    }
    IEnumerator DisableEffect()
    {
        yield return new WaitForSeconds(1f);
        skillImage.sprite = skillDisabledSprite;
    }
    private void LightSkill()
    {
        if ( isLevel2BossFight )
            radioActivacion = 4;
        else radioActivacion = 15;
        magicCircleActivatePlatformPS.Play();
        explosionActivatePlatformPS.Play();

        Collider[] colliders = Physics.OverlapSphere(transform.position, radioActivacion, capaPlataformas);

        foreach (Collider collider in colliders)
        {
            invis_platf plataforma = collider.GetComponent<invis_platf>();
            TorchBossL2 torchBossL2 = collider.GetComponent<TorchBossL2>();

            if (plataforma != null)
            {
                AudioManager.instance.PlayClip(SoundsFX.SFX_Player_Hability);
                plataforma.Activar();
            }
            if (torchBossL2)
            {
                torchBossL2.Activar();
            }
        }
    }
    public void ActivateWindSkill(Vector3 position, float activationRadius)
    {
        Collider[] colliders = Physics.OverlapSphere(position, activationRadius);
        bool wind = false;
        foreach (Collider collider in colliders)
        {
            Box_behaviour box = collider.GetComponent<Box_behaviour>();
            if (box != null)
            {
                skillImage.sprite = airSkillEnabledSprite;

                box.Move(transform.forward, wind_dist);
                wind = true;
            }
        }
        if (wind)
        {
            StartCoroutine(DisableEffect());

        }
    }
    private void FireSkill()
    {
        animator.SetTrigger("IsShoot");

        // Retrasamos el lanzamiento de la bola de fuego
        float delay = CalculateDelay(); // Calculamos el retardo necesario
        Invoke("LaunchFireball", delay); // Invocamos el método 'LaunchFireball' después del retardo
        skillImage.sprite = fireSkillEnabledSprite;

    }

    private float CalculateDelay()
    {
        // Calculamos el tiempo de retardo como la mitad de la duración de la animación
        float animationDuration = 0.8f;
        return animationDuration / 2f;
    }

    private void LaunchFireball()
    {
        shootPS.Play();
        GameObject fireball = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
        Rigidbody rb = fireball.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = transform.forward * fireballSpeed; // Aplicamos la velocidad definida
        }
        Destroy(fireball, 5f); // Puedes ajustar el tiempo de vida de la bola de fuego según tus necesidades

        // Iniciamos el cooldown después de lanzar la bola de fuego
        StartCooldown();
    }
    public void EnableFire()
    {
        fireEnabled= true;
    }
    void StartCooldown()
    {
        isCooldown = true;
        cooldownTimer = cooldownDuration; // Establecemos el temporizador al valor de la duración del cooldown
    }
}
