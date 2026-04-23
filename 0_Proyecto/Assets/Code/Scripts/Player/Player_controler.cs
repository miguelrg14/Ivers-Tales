using RenownedGames.AITree.Demo;
using RenownedGames.Apex;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.VFX;
using TMPro;

public class Player_controler : MonoBehaviour
{
    [Group("Movement")]
    [SerializeField]
    float moveSpeed = 5.0f;
    [Group("Movement")]
    [SerializeField]
    float decelerationSpeed = 5f;
    [Group("Movement")]
    [SerializeField]
    float frenado = 5f;

    [Group("Dash")]
    [SerializeField]
    float dashDistance = 5.0f;
    [Group("Dash")]
    [SerializeField]
    float dashDuration = 0.3f;
    [Group("Dash")]
    [SerializeField]
    float dashForce = 10f;
    [Group("Dash")]
    [SerializeField]
    float dashForceForAttacks = 15f;
    [Group("Dash")]
    [SerializeField]
    bool isDashing;
    bool startDash;
    [Group("References")]
    [SerializeField]
    ParticleSystem dashPS;

    [Group("Jump")]
    [SerializeField]
    float jumpForce = 1.0f;

    Rigidbody rb;
    Vector3 dashStartPosition;
    Vector3 dashEndPosition;
    float dashStartTime;
    bool isGrounded;
    bool isInvincible = false;
    Animator animator;
    bool isRunning = false;
    bool isRecievingAttack = false;

    [Group("Attack")]
    [SerializeField]
    public bool isAttack;

    [Group("Health")]
    [SerializeField]
    private bool enableLifes;
    [Group("Health")]
    [SerializeField]
    private int health = 40;
    [Group("Health")]
    [SerializeField]
    private Image heart1;
    [Group("Health")]
    [SerializeField]
    private Image heart2;
    [Group("Health")]
    [SerializeField]
    private Image heart3;
    [Group("Health")]
    [SerializeField]
    private Image heart4;
    [Group("Health")]
    [SerializeField]
    private Sprite heartDissabled;

    [Group("UI_gems")]
    [SerializeField]
    int gems_count = 0;
    public TextMeshProUGUI UI_gems;

    [Group("Dash")]
    [SerializeField]
    private Image dashImage;

    [Group("Dash")]
    [SerializeField]
    private Sprite dashDisabledSprite;

    [Group("Dash")]
    [SerializeField]
    private Sprite dashEnabledSprite;

    [Group("Dependencies Level")]
    [SerializeField]
    private restart restart;
    private VFXManager vfxManager;



    [Group("IA Lucas")]
    [SerializeField]
    bool allowsMovementPrediction;
    private HistoricalMovementPlayer historicalMovementPlayer;
    private NavMeshObstacle navMeshObstacle;

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
    AudioClip dash_clip;
    [Group("Audio")]
    [SerializeField]
    AudioClip death_clip;

    [SerializeField]
    Animator animatorCam;
    // Establece el fps objetivo (por ejemplo, 60 fps)
    public int targetFramerate = 60;
    [Group("Hability")]
    [SerializeField]
    public KeyCode teclaActivacion = KeyCode.V;
    private float dashForceFieldBossL2;
    bool isRecievingAttackForceFieldBossL2 = false;

    void Start()
    {
        // Establece el fps objetivo al iniciar el juego
        Application.targetFrameRate = targetFramerate;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        navMeshObstacle = GetComponent<NavMeshObstacle>();
        animator = GetComponentInChildren<Animator>();
        vfxManager=FindObjectOfType<VFXManager>();  //TODO : needs to be actualized to a way of chasing the reference
        if (allowsMovementPrediction)
            historicalMovementPlayer = GetComponent<HistoricalMovementPlayer>();
    }

    void Update()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);

        if (!isDashing)
        {
            // Movimiento
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            Vector3 moveDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;

            if (Input.GetButtonDown("Dash"))
            {
                if (CanDash())
                {
                    StartDash(moveDirection);
                    startDash = true;
                    audioSource.PlayOneShot(dash_clip);
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (!isDashing)
        {
            // Movimiento
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            Vector3 moveDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;
            
            // Gira el personaje hacia la dirección del movimiento
            if (moveDirection != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(moveDirection);
            }

            if (!isAttack)
            {
                rb.velocity = new Vector3(moveDirection.x * moveSpeed, rb.velocity.y, moveDirection.z * moveSpeed);
                if (moveDirection != Vector3.zero)
                {
                    StartRun();
                }
                else
                {
                    StopRun();
                    //Debug.Log("parao");
                }
            }

            //// Salto
            //if (isGrounded && Input.GetButtonDown("Jump"))
            //{
            //    rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            //}

            // Dash
            //if (Input.GetButtonDown("Dash"))
            //{
            //    if (CanDash())
            //    {
            //        rb.AddForce(Vector3.up * jumpForce , ForceMode.Impulse);
            //        StartDash(moveDirection);
            //        audioSource.PlayOneShot(dash_clip);
            //    }
            //}
        }
        else
        {
            if (isRecievingAttack)
                PerformDash(dashForceForAttacks);
            else if (isRecievingAttackForceFieldBossL2) PerformDash(dashForceFieldBossL2);
            //else if (startDash)
            //PerformJump();

            else
                PerformDash();

        }

    }

    void PerformJump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        startDash = false;
    }
    public void add_gems()
    {
        gems_count++;
        UI_gems.text="x"+gems_count.ToString();

    }




    bool CanDash()                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             
    {
        if (isGrounded)
        {
            return true;
        }
        return false;
    }
    void StartRun()
    {        
        isRunning = true;

        //Debug.Log("corriendo");
        animator.SetFloat("XSpeed", 1);
            animator.SetBool("IsRunning", true);
        
    }

    public void StopRun()
    {
        isRunning = false;

        if (animator != null)
        {
            animator.SetFloat("XSpeed", 0);
            animator.SetBool("IsRunning", false);
        }
        //rb.velocity = new Vector3(0, transform.position.y, 0);

    }
    void StartDash(Vector3 dashDirection)
    {
        dashImage.sprite = dashEnabledSprite;
        dashPS.Play();
        isDashing = true;
        dashStartPosition = transform.position;
        // Agregar un pequeño impulso vertical
        float verticalImpulse = 0.5f; // Modifica este valor según sea necesario
        Vector3 dashDirectionWithImpulse = dashDirection.normalized + Vector3.up * verticalImpulse;
        dashEndPosition = transform.position + dashDirection.normalized * dashDistance;
        dashStartTime = Time.fixedTime;
        // Activa la invulnerabilidad durante el dash.
        isInvincible = true;
        // Activa la animación de dash
        if (animator != null)
        {
            animator.SetTrigger("DashTrigger"); 
        }
    }
    void StartDashWhitoutanim(Vector3 dashDirection)
    {
        isDashing = true;
        dashStartPosition = transform.position;
        dashEndPosition = transform.position + dashDirection.normalized * dashDistance;
        dashStartTime = Time.fixedTime;
        // Activa la invulnerabilidad durante el dash.
        isInvincible = true;
        // Activa la animación de dash
        if (animator != null)
        {
            animator.SetTrigger("Damaged");
        }
    }

    void PerformDash()
    {

        //float dashProgress = (Time.fixedTime - dashStartTime) / dashDuration;

        //if (dashProgress < 1.0f)
        //{
        //    // Interpola suavemente la posición del personaje durante el dash.
        //    // Utiliza la curva de interpolación para un movimiento más suave 
        //    //float interpolation = dashCurve.Evaluate(dashProgress);

        //    // Calcula la dirección del dash y aplica un impulso al Rigidbody.
        //    Vector3 dashDirection = (dashEndPosition - dashStartPosition).normalized;
        //    rb.AddForce(dashDirection * dashForce /***/ /*interpolation*/, ForceMode.Impulse);
        //}
        //else
        //{
        //    // Finaliza el dash y restablece el movimiento normal.
        //    isDashing = false;
        //    isInvincible = false;

        //    rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, decelerationSpeed * Time.fixedDeltaTime);
        //}
        float dashProgress = (Time.fixedTime - dashStartTime) / dashDuration;

        if (dashProgress < 1.0f)
        {
            // Calcula la dirección del dash en el plano horizontal
            Vector3 dashDirection = (dashEndPosition - dashStartPosition).normalized;
            dashDirection.y = 0; // Ignora completamente el movimiento vertical

            // Aplica un impulso al Rigidbody en la dirección del dash
            rb.AddForce(dashDirection * dashForce, ForceMode.Impulse);

            // Aplica un pequeño impulso vertical para mantener al jugador pegado al suelo
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
        else
        {
            // Finaliza el dash y restablece el movimiento normal.
            isDashing = false;
            isInvincible = false;

            // Detiene completamente el movimiento en el eje Y para evitar cualquier elevación o caída
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            dashImage.sprite = dashDisabledSprite;

        }
    }
    void PerformDash(float strengthDash)
    {
        float dashProgress = (Time.fixedTime - dashStartTime) / dashDuration;


        if (dashProgress < 1.0f)
        {
            // Interpola suavemente la posición del personaje durante el dash.
            // Utiliza la curva de interpolación para un movimiento más suave 
            //float interpolation = dashCurve.Evaluate(dashProgress);

            // Calcula la dirección del dash y aplica un impulso al Rigidbody.
            Vector3 dashDirection = (dashEndPosition - dashStartPosition).normalized;
            rb.AddForce(dashDirection * strengthDash /***/ /*interpolation*/, ForceMode.Impulse);
        }
        else
        {
            // Finaliza el dash y restablece el movimiento normal.
            isDashing = false;
            isInvincible = false;
            isRecievingAttack = false;
            isRecievingAttackForceFieldBossL2 = false;
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, decelerationSpeed * Time.fixedDeltaTime);
        }
    }
    public Vector3 ReturnVelocityRigidbody()
    {
        return rb.velocity;
    }
    public Vector3 ReturnAverageVelocity()
    {
        return historicalMovementPlayer.averageVelocity;
    }
    public bool IsPlayerMoving()
    {
        return historicalMovementPlayer.CompareTheLastVector3sPosition();
    }

    void OnCollisionEnter(Collision collision)
    {
        // Verifica si el jugador está en el suelo y no es invulnerable.
        if (collision.collider.CompareTag("Ground") && !isInvincible)
        {
            isDashing = false;
            isInvincible = false;
        }      
    }
    private void OnTriggerEnter(Collider other)
    {
        RecieveDamageFromBullets(other);
    }
    public void OnDamageFromShortDistanceEnemy(ShortDistanceEnemy enemy)
    {
        if (enemy.isActiveAndEnabled)
        {
            //animatorCam.SetTrigger("EnableZoom");

            Vector3 positionForRecievingDamage = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
            vfxManager.ActivateVFXPlayerRecieveDamage(positionForRecievingDamage, Quaternion.identity);
            OnDamage((int)enemy.damage);
            Vector3 impactDirection = transform.position - enemy.transform.position;
            impactDirection.Normalize();
            isRecievingAttack = true;
            StartDashWhitoutanim(impactDirection);
        }

    }
    public void ActivateDesactivateNavMeshObstacle(bool isActivatedOneEnemyMode)
    {
        if (!isActivatedOneEnemyMode)
            navMeshObstacle.enabled=true;
        else
            navMeshObstacle.enabled = false;
    }
    public void RecieveDamageFromForceField(Vector3 direction,float dashForceField)
    {
        AudioManager.instance.PlayClip(SoundsFX.SFX_FireContact);
        isRecievingAttackForceFieldBossL2 = true;
        this.dashForceFieldBossL2= dashForceField;
        StartDashWhitoutanim(direction);

    }
    public void RecieveDamageFromBullets(Collider other)
    {
        if (other.tag == "BulletMortar")
        {
            //animatorCam.SetTrigger("EnableZoom");
            AudioManager.instance.PlayClip(SoundsFX.SFX_FireContact);
            BulletLongDistanceEnemy bullet = other.GetComponentInParent<BulletLongDistanceEnemy>();
            if (bullet != null)
            {
                Debug.Log("Getting Damage from bullet final boss");
                OnDamage(bullet.GetBulletDamage());
                if (!isDashing)
                {
                    Vector3 impactDirection = transform.position - other.transform.position;
                    impactDirection.Normalize();
                    StartDashWhitoutanim(impactDirection);
                }
                bullet.ResetValues();
                bullet.DesactivateDiana();
            }
        }
        else if (other.tag == "Bullet")
        {

            AudioManager.instance.PlayClip(SoundsFX.SFX_FireContact);
            BulletFinalBoss bullet = other.GetComponentInParent<BulletFinalBoss>();
            if (bullet != null)
            {
                Debug.Log("Getting Damage from bullet final boss");
                OnDamage(bullet.GetBulletDamage());
                if (!isDashing)
                {
                    Vector3 impactDirection = transform.position - other.transform.position;
                    impactDirection.Normalize();
                    StartDashWhitoutanim(impactDirection);
                }
                bullet.ResetValues();

            }
        }
        else if (other.tag == "BulletTornado")
        {
            AudioManager.instance.PlayClip(SoundsFX.SFX_FireContact);
            BulletFinalBoss bullet = other.GetComponentInParent<BulletFinalBoss>();
            if (bullet != null)
            {
                Debug.Log("Getting Damage from bullet final boss");
                OnDamage(bullet.GetBulletDamage());
                if (!isDashing)
                {
                    Vector3 impactDirection = transform.position - other.transform.position;
                    impactDirection.Normalize();
                    vfxManager.ActivateVFXPlayerRecieveDamageFromEnemyTornado(transform.position, Quaternion.identity);
                    StartDashWhitoutanim(impactDirection);
                }
                bullet.ResetValues();

            }
        }
    }
    public void OnDamage(int damage)
    {
        Vector3 positionForRecievingDamage = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        vfxManager.ActivateVFXPlayerRecieveDamage(positionForRecievingDamage,Quaternion.identity);
        health-=damage;

        audioSource.PlayOneShot(hurt_clip);

        if (enableLifes)
        {
            if (health == 30)
            {
                heart4.sprite = heartDissabled;

            }
            if (health == 20)
            {
                heart3.sprite = heartDissabled;

            }
            if (health == 10)
            {
                heart2.sprite = heartDissabled;

            }
            if (health == 0)
            {
                heart1.sprite = heartDissabled;

                audioSource.PlayOneShot(death_clip);

                if (animator != null)
                {
                    animator.SetTrigger("Die");
                }
                //heart1.enabled = true;
                //heart2.enabled = true;
                //heart3.enabled = true;
                //heart4.enabled = true;
                //health = 0;
                StartCoroutine(WaitAndInvokeEvent());
               

            }
        }

        Debug.Log("gets damage the player");
        //next is handle death and all this stuff


    }
    IEnumerator WaitAndInvokeEvent()
    {
        yield return new WaitForSeconds(1.4f); // Espera 2 segundos
        restart.RestartGame();

    }
}
