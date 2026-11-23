using System.Collections;
using IversTales.Core;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// Iver's three spells, selected with Sk1/Sk2/Sk3 and cast with "Skill":
/// 1. Light: reveals the hidden platforms (and lights the level 2 boss torches) around the player.
/// 2. Fire: shoots a fireball (unlocked during the game, has a cooldown).
/// 3. Wind: pushes the boxes in front of the player.
/// </summary>
public class PlayerSkills : MonoBehaviour
{
    enum Spell { None = 0, Light = 1, Fire = 2, Wind = 3 }

    [Header("Light spell")]
    [FormerlySerializedAs("capaPlataformas")] public LayerMask platformLayer;
    [FormerlySerializedAs("radioActivacion")] public float activationRadius = 5f;
    [SerializeField] float lightRadius = 15f;
    [SerializeField] float lightRadiusInBossFight = 4f;
    [SerializeField] ParticleSystem explosionActivatePlatformPS;
    [SerializeField] ParticleSystem magicCircleActivatePlatformPS;

    [Header("Fire spell")]
    public GameObject fireballPrefab;
    public Transform firePoint;
    public bool fireEnabled = false;
    public float fireballSpeed = 10f;
    public int fireballDamage = 10;
    [Tooltip("Seconds between two fireballs.")]
    public float cooldownDuration = 2f;
    [Tooltip("Seconds between the cast animation starting and the fireball leaving the hand.")]
    [SerializeField] float fireballLaunchDelay = 0.4f;
    [SerializeField] float fireballLifetime = 5f;
    [SerializeField] ParticleSystem shootPS;

    [Header("Wind spell")]
    [FormerlySerializedAs("wind_pos")] public Transform windOrigin;
    [FormerlySerializedAs("wind_dist")] public float windDistance = 2;
    [SerializeField] float windRadius = 2f;

    [Header("State")]
    [Tooltip("Selected spell: 1 light, 2 fire, 3 wind.")]
    public float index = 1;
    public bool isLevel2BossFight = false;
    public Animator animator;

    [Header("HUD")]
    [SerializeField] Image skillImage;
    [SerializeField] Sprite lightSkillEnabledSprite;
    [SerializeField] Sprite fireSkillEnabledSprite;
    [SerializeField] Sprite airSkillEnabledSprite;
    [SerializeField] Sprite skillDisabledSprite;
    [SerializeField] float hudHighlightDuration = 1f;

    [Header("Audio")]
    [SerializeField] AudioSource audioSource;
    [SerializeField, FormerlySerializedAs("hability_platform_clip")] AudioClip platformAbilityClip;
    [SerializeField, FormerlySerializedAs("hability_fire_clip")] AudioClip fireAbilityClip;
    [SerializeField, FormerlySerializedAs("hability_wind_clip")] AudioClip windAbilityClip;

    Cooldown fireCooldown;
    Coroutine hudHighlight;

    Spell Selected => (Spell)Mathf.RoundToInt(index);

    void Awake()
    {
        fireCooldown = new Cooldown(cooldownDuration);
    }

    void Update()
    {
        fireCooldown.Duration = cooldownDuration;
        fireCooldown.Tick(Time.deltaTime);

        if (Input.GetButtonDown("Sk1")) Select(Spell.Light, lightSkillEnabledSprite);
        if (Input.GetButtonDown("Sk2")) Select(Spell.Fire, fireSkillEnabledSprite);
        if (Input.GetButtonDown("Sk3")) index = (int)Spell.Wind;
        if (Input.GetButtonDown("Skill")) CastSelected();
    }

    void Select(Spell spell, Sprite hudSprite)
    {
        index = (int)spell;
        HighlightHud(hudSprite);
    }

    void CastSelected()
    {
        switch (Selected)
        {
            case Spell.Light:
                HighlightHud(lightSkillEnabledSprite);
                animator.SetTrigger("Skill1");
                LightSkill();
                PlayOneShot(platformAbilityClip);
                break;

            case Spell.Fire:
                if (fireEnabled && fireCooldown.TryTrigger())
                {
                    HighlightHud(fireSkillEnabledSprite);
                    PlayOneShot(fireAbilityClip);
                    FireSkill();
                }
                break;

            case Spell.Wind:
                PlayOneShot(windAbilityClip);
                ActivateWindSkill(windOrigin.position, windRadius);
                break;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, activationRadius);
        if (windOrigin != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(windOrigin.position, windRadius);
        }
    }

    void HighlightHud(Sprite sprite)
    {
        if (skillImage == null) return;
        skillImage.sprite = sprite;
        if (hudHighlight != null) StopCoroutine(hudHighlight);
        hudHighlight = StartCoroutine(ResetHudAfterDelay());
    }

    IEnumerator ResetHudAfterDelay()
    {
        yield return new WaitForSeconds(hudHighlightDuration);
        skillImage.sprite = skillDisabledSprite;
        hudHighlight = null;
    }

    void LightSkill()
    {
        activationRadius = isLevel2BossFight ? lightRadiusInBossFight : lightRadius;
        magicCircleActivatePlatformPS.Play();
        explosionActivatePlatformPS.Play();

        foreach (Collider collider in Physics.OverlapSphere(transform.position, activationRadius, platformLayer))
        {
            var platform = collider.GetComponent<HiddenPlatform>();
            if (platform != null)
            {
                AudioManager.Instance?.PlayClip(SoundsFX.SFX_Player_Ability);
                platform.Activate();
            }

            var torch = collider.GetComponent<TorchBossL2>();
            if (torch != null) torch.Activate();
        }
    }

    public void ActivateWindSkill(Vector3 position, float radius)
    {
        bool pushedSomething = false;
        foreach (Collider collider in Physics.OverlapSphere(position, radius))
        {
            var box = collider.GetComponent<PushableBox>();
            if (box == null) continue;
            box.Move(transform.forward, windDistance);
            pushedSomething = true;
        }
        if (pushedSomething) HighlightHud(airSkillEnabledSprite);
    }

    void FireSkill()
    {
        animator.SetTrigger("IsShoot");
        // Wait for the hand to reach the front of the cast animation before spawning the fireball.
        Invoke(nameof(LaunchFireball), fireballLaunchDelay);
    }

    void LaunchFireball()
    {
        shootPS.Play();
        GameObject fireball = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
        if (fireball.TryGetComponent(out Rigidbody body))
            body.velocity = transform.forward * fireballSpeed;
        Destroy(fireball, fireballLifetime);
    }

    /// <summary>Unlocks the fire spell (called when the player picks up the fire gem).</summary>
    public void EnableFire() => fireEnabled = true;

    void PlayOneShot(AudioClip clip)
    {
        if (audioSource != null && clip != null) audioSource.PlayOneShot(clip);
    }
}
