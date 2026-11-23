using System.Collections;
using IversTales.Core;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// Iver's movement, dash, health (hearts), gem counter and reactions to enemy hits.
/// </summary>
public class PlayerController : MonoBehaviour
{
    const int HealthPerHeart = 10;

    [Header("Movement")]
    [SerializeField] float moveSpeed = 5.0f;
    [SerializeField] float decelerationSpeed = 5f;

    [Header("Dash")]
    [SerializeField] float dashDistance = 5.0f;
    [SerializeField] float dashDuration = 0.3f;
    [SerializeField] float dashForce = 10f;
    [Tooltip("Force of the knock-back dash when an enemy hits the player.")]
    [SerializeField] float dashForceForAttacks = 15f;
    [SerializeField] bool isDashing;
    [SerializeField] Image dashImage;
    [SerializeField] Sprite dashDisabledSprite;
    [SerializeField] Sprite dashEnabledSprite;

    [Header("References")]
    [SerializeField] ParticleSystem dashPS;

    [Header("Jump")]
    [Tooltip("Small upward impulse applied while dashing so the player does not stick to the floor.")]
    [SerializeField] float jumpForce = 1.0f;

    [Header("Attack")]
    [Tooltip("Set by the melee attack while it is playing; blocks movement.")]
    public bool isAttack;

    [Header("Health")]
    [Tooltip("Disable to make the player invulnerable while testing.")]
    [SerializeField, FormerlySerializedAs("enableLifes")] bool healthEnabled = true;
    [SerializeField, FormerlySerializedAs("health")] int maxHealth = 40;
    [Tooltip("Hearts in the HUD, from the last one to lose (1) to the first one (4).")]
    [SerializeField] Image heart1;
    [SerializeField] Image heart2;
    [SerializeField] Image heart3;
    [SerializeField] Image heart4;
    [SerializeField, FormerlySerializedAs("heartDissabled")] Sprite heartDisabledSprite;

    [Header("Gems")]
    [SerializeField, FormerlySerializedAs("gems_count")] int gemCount = 0;
    [FormerlySerializedAs("UI_gems")] public TextMeshProUGUI gemCounterText;

    [Header("Level")]
    [SerializeField, FormerlySerializedAs("restart")] LevelRestarter levelRestarter;
    [SerializeField] float restartDelayAfterDeath = 1.4f;

    [Header("Enemy AI")]
    [Tooltip("Records the player's recent movement so ranged enemies can predict where to shoot.")]
    [SerializeField] bool allowsMovementPrediction;

    [Header("Audio")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip hurt_clip;
    [SerializeField] AudioClip dash_clip;
    [SerializeField] AudioClip death_clip;

    Rigidbody rb;
    Animator animator;
    VFXManager vfxManager;
    HistoricalMovementPlayer historicalMovementPlayer;
    NavMeshObstacle navMeshObstacle;
    PlayerHealth health;
    Image[] hearts;

    Vector3 dashStartPosition;
    Vector3 dashEndPosition;
    float dashStartTime;
    bool isGrounded;
    bool isInvincible;
    bool isReceivingAttack;
    bool isReceivingAttackForceFieldBossL2;
    float dashForceFieldBossL2;

    public bool IsDead => health != null && health.IsDead;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        navMeshObstacle = GetComponent<NavMeshObstacle>();
        animator = GetComponentInChildren<Animator>();
        vfxManager = FindObjectOfType<VFXManager>();
        if (allowsMovementPrediction)
            historicalMovementPlayer = GetComponent<HistoricalMovementPlayer>();

        health = new PlayerHealth(Mathf.Max(1, maxHealth), HealthPerHeart);
        health.Died += OnDied;
        hearts = new[] { heart1, heart2, heart3, heart4 };
    }

    void Update()
    {
        if (IsDead) return;
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);

        if (!isDashing && Input.GetButtonDown("Dash") && isGrounded)
        {
            StartDash(ReadMoveDirection());
            PlayOneShot(dash_clip);
        }
    }

    void FixedUpdate()
    {
        if (IsDead) return;

        if (isDashing)
        {
            if (isReceivingAttack) PerformDash(dashForceForAttacks);
            else if (isReceivingAttackForceFieldBossL2) PerformDash(dashForceFieldBossL2);
            else PerformDash();
            return;
        }

        Vector3 moveDirection = ReadMoveDirection();

        // Face the direction of movement.
        if (moveDirection != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(moveDirection);

        if (!isAttack)
        {
            rb.velocity = new Vector3(moveDirection.x * moveSpeed, rb.velocity.y, moveDirection.z * moveSpeed);
            if (moveDirection != Vector3.zero) StartRun();
            else StopRun();
        }
    }

    static Vector3 ReadMoveDirection() =>
        new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized;

    // ---- Gems ----

    public void AddGem()
    {
        gemCount++;
        if (gemCounterText != null) gemCounterText.text = "x" + gemCount;
    }

    // ---- Running / dash ----

    void StartRun()
    {
        animator.SetFloat("XSpeed", 1);
        animator.SetBool("IsRunning", true);
    }

    public void StopRun()
    {
        if (animator == null) return;
        animator.SetFloat("XSpeed", 0);
        animator.SetBool("IsRunning", false);
    }

    void StartDash(Vector3 dashDirection)
    {
        if (dashImage != null) dashImage.sprite = dashEnabledSprite;
        if (dashPS != null) dashPS.Play();
        BeginDash(dashDirection);
        if (animator != null) animator.SetTrigger("DashTrigger");
    }

    /// <summary>Pushes the player away from a hit, reusing the dash movement.</summary>
    void StartKnockback(Vector3 direction)
    {
        BeginDash(direction);
        if (animator != null) animator.SetTrigger("Damaged");
    }

    void BeginDash(Vector3 direction)
    {
        isDashing = true;
        isInvincible = true; // invulnerable while dashing
        dashStartPosition = transform.position;
        dashEndPosition = transform.position + direction.normalized * dashDistance;
        dashStartTime = Time.fixedTime;
    }

    void PerformDash()
    {
        float dashProgress = (Time.fixedTime - dashStartTime) / dashDuration;
        if (dashProgress < 1.0f)
        {
            // Horizontal impulse in the dash direction plus a small upward one.
            Vector3 dashDirection = (dashEndPosition - dashStartPosition).normalized;
            dashDirection.y = 0;
            rb.AddForce(dashDirection * dashForce, ForceMode.Impulse);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
        else
        {
            EndDash();
            // Cancel vertical speed so the dash never launches the player upwards.
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            if (dashImage != null) dashImage.sprite = dashDisabledSprite;
        }
    }

    void PerformDash(float strength)
    {
        float dashProgress = (Time.fixedTime - dashStartTime) / dashDuration;
        if (dashProgress < 1.0f)
        {
            Vector3 dashDirection = (dashEndPosition - dashStartPosition).normalized;
            rb.AddForce(dashDirection * strength, ForceMode.Impulse);
        }
        else
        {
            EndDash();
            isReceivingAttack = false;
            isReceivingAttackForceFieldBossL2 = false;
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, decelerationSpeed * Time.fixedDeltaTime);
        }
    }

    void EndDash()
    {
        isDashing = false;
        isInvincible = false;
    }

    // ---- Data for the enemy AI ----

    public Vector3 ReturnVelocityRigidbody() => rb.velocity;
    public Vector3 ReturnAverageVelocity() => historicalMovementPlayer.averageVelocity;
    public bool IsPlayerMoving() => historicalMovementPlayer.CompareTheLastVector3sPosition();

    public void ToggleNavMeshObstacle(bool isActivatedOneEnemyMode)
    {
        navMeshObstacle.enabled = !isActivatedOneEnemyMode;
    }

    // ---- Damage ----

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ground") && !isInvincible)
            EndDash();
    }

    void OnTriggerEnter(Collider other) => ReceiveDamageFromBullets(other);

    public void OnDamageFromShortDistanceEnemy(ShortDistanceEnemy enemy)
    {
        if (!enemy.isActiveAndEnabled) return;
        OnDamage((int)enemy.damage);
        isReceivingAttack = true;
        StartKnockback((transform.position - enemy.transform.position).normalized);
    }

    public void ReceiveDamageFromForceField(Vector3 direction, float dashForceField)
    {
        AudioManager.Instance?.PlayClip(SoundsFX.SFX_FireContact);
        isReceivingAttackForceFieldBossL2 = true;
        dashForceFieldBossL2 = dashForceField;
        StartKnockback(direction);
    }

    public void ReceiveDamageFromBullets(Collider other)
    {
        if (other.CompareTag("BulletMortar"))
        {
            var bullet = other.GetComponentInParent<BulletLongDistanceEnemy>();
            if (bullet == null) return;
            AudioManager.Instance?.PlayClip(SoundsFX.SFX_FireContact);
            OnDamage(bullet.GetBulletDamage());
            KnockbackFrom(other.transform.position);
            bullet.ResetValues();
            bullet.DeactivateTargetMarker();
        }
        else if (other.CompareTag("Bullet") || other.CompareTag("BulletTornado"))
        {
            var bullet = other.GetComponentInParent<BulletFinalBoss>();
            if (bullet == null) return;
            AudioManager.Instance?.PlayClip(SoundsFX.SFX_FireContact);
            OnDamage(bullet.GetBulletDamage());
            if (!isDashing && other.CompareTag("BulletTornado"))
                vfxManager.ActivateVFXPlayerReceiveDamageFromEnemyTornado(transform.position, Quaternion.identity);
            KnockbackFrom(other.transform.position);
            bullet.ResetValues();
        }
    }

    void KnockbackFrom(Vector3 hitPosition)
    {
        if (!isDashing)
            StartKnockback((transform.position - hitPosition).normalized);
    }

    public void OnDamage(int damage)
    {
        if (IsDead) return;

        vfxManager.ActivateVFXPlayerReceiveDamage(transform.position + Vector3.up, Quaternion.identity);
        PlayOneShot(hurt_clip);

        if (!healthEnabled) return;
        health.TakeDamage(damage);
        UpdateHearts();
    }

    void UpdateHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
            if (hearts[i] != null && i >= health.VisibleHearts)
                hearts[i].sprite = heartDisabledSprite;
    }

    void OnDied()
    {
        PlayOneShot(death_clip);
        if (animator != null) animator.SetTrigger("Die");
        rb.velocity = Vector3.zero;
        StartCoroutine(RestartAfterDeath());
    }

    IEnumerator RestartAfterDeath()
    {
        yield return new WaitForSeconds(restartDelayAfterDeath);
        if (levelRestarter != null) levelRestarter.RestartGame();
        else UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    void PlayOneShot(AudioClip clip)
    {
        if (audioSource != null && clip != null) audioSource.PlayOneShot(clip);
    }
}
