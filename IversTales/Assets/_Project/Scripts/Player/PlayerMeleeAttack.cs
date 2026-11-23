using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// Hammer attack: on "Attack" (with cooldown) plays the animation and VFX and
/// damages every enemy inside a sphere in front of Iver.
/// </summary>
public class PlayerMeleeAttack : MonoBehaviour
{
    const int EnemyDamage = 100;
    const int BossDamage = 25;

    public float attackCooldown = 1f;
    public Transform attackPoint;
    public float attackRange = 1f;
    public Animator animator;
    public PlayerController player;

    [Tooltip("True while the attack VFX is playing.")]
    [FormerlySerializedAs("animacionEnCurso")] public bool attackInProgress = false;
    [FormerlySerializedAs("tiempoInicioAnimacion")] public float attackStartTime;

    [Header("References")]
    [SerializeField] ParticleSystem attackPS;

    [Header("Audio")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip attack_clip;

    [Header("HUD")]
    [SerializeField] Image attackImage;
    [SerializeField] Sprite attackEnabledSprite;
    [SerializeField] Sprite attackDisabledSprite;

    float lastAttackTime;

    void Start()
    {
        lastAttackTime = -attackCooldown;
    }

    void Update()
    {
        if (player != null && player.IsDead) return;

        if (Input.GetButtonDown("Attack") && Time.time - lastAttackTime > attackCooldown)
            Attack();

        if (attackInProgress && !attackPS.isPlaying)
            EndAttack();
    }

    void Attack()
    {
        lastAttackTime = Time.time;
        if (attackImage != null) attackImage.sprite = attackEnabledSprite;
        if (audioSource != null && attack_clip != null) audioSource.PlayOneShot(attack_clip);

        player.StopRun();
        StartAttack();
        animator.SetTrigger("attack");

        foreach (Collider hit in Physics.OverlapSphere(attackPoint.position, attackRange))
            if (hit.CompareTag("Enemy"))
                DamageEnemy(hit);

        player.isAttack = false;
    }

    static void DamageEnemy(Collider hit)
    {
        var shortDistance = hit.GetComponentInParent<ShortDistanceEnemy>();
        if (shortDistance != null) { shortDistance.ReceiveDamage(EnemyDamage); return; }

        var bossLevel1 = hit.GetComponentInParent<FinalBossLevelOne>();
        if (bossLevel1 != null) { bossLevel1.ReceiveDamage(BossDamage); return; }

        var bossLevel2 = hit.GetComponentInParent<FinalBossLevelTwo>();
        if (bossLevel2 != null) { bossLevel2.ReceiveDamage(BossDamage); return; }

        var tornado = hit.GetComponentInParent<TornadoEnemy>();
        if (tornado != null) { tornado.ReceiveDamage(EnemyDamage); return; }

        var longDistance = hit.GetComponentInParent<LongDistanceEnemy>();
        if (longDistance != null) longDistance.ReceiveDamage(EnemyDamage);
    }

    void StartAttack()
    {
        attackPS.Play();
        attackInProgress = true;
        attackStartTime = Time.time;
    }

    void EndAttack()
    {
        attackInProgress = false;
        player.isAttack = false;
        if (attackImage != null) attackImage.sprite = attackDisabledSprite;
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
