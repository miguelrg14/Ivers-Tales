using RenownedGames.AITree.Demo;
using RenownedGames.Apex;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bad_combat : MonoBehaviour
{

    public float attackCooldown = 1f;
    public Transform attackPoint;
    public float attackRange = 1f;
    private float duracionAnimacion = 1.2f; 

    public bool animacionEnCurso = false;
    public float tiempoInicioAnimacion;
    private Rigidbody rb;
    public  Animator animator;
    private bool isAttacking = false;
    private float lastAttackTime;
    public Player_controler player;

    [Group("References")]
    [SerializeField]
    ParticleSystem attackPS;

    [Group("Audio")]
    [SerializeField]
    private AudioSource audioSource;
    [Group("Audio")]
    [SerializeField]
    AudioClip attack_clip;

    [Group("Attack")]
    [SerializeField]
    private Image attackImage;

    [Group("Attack")]
    [SerializeField]
    private Sprite attackEnabledSprite;

    [Group("Attack")]
    [SerializeField]
    private Sprite attackDisabledSprite;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        lastAttackTime = -attackCooldown;
    }

    private void Update()
    {        
        HandleAttackInput();
        if (animacionEnCurso && !attackPS.isPlaying)
        {
            EndAttack();
        }
    }
    void HandleAttackInput()
    {
        if (Input.GetButtonDown("Attack") && Time.time - lastAttackTime > attackCooldown)
        {
            Attack();
        }
    }

    void Attack()
    {
        attackImage.sprite = attackEnabledSprite;
        lastAttackTime = Time.time;

        audioSource.PlayOneShot(attack_clip);


        // Inicia la animación de ataque
        player.StopRun();
        StartAttack();
        animator.SetTrigger("attack");


        // Lógica de detección de impactos con el arma
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange);

        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.tag == "Enemy")
            {
                // Realiza acciones específicas cuando se detecta un enemigo dentro del rango de ataque
                ShortDistanceEnemy enemyController = enemy.GetComponentInParent<ShortDistanceEnemy>();
                FinalBossLevelOne bossController = enemy.GetComponentInParent<FinalBossLevelOne>();
                FinalBossLevelTwo bossController2 = enemy.GetComponentInParent<FinalBossLevelTwo>();

                TornadoEnemy tornadoEnemy =enemy.GetComponentInParent<TornadoEnemy>();   
                if (enemyController != null)
                {
                    enemyController.RecieveDamage(100);
                }
                else if (bossController!=null)
                {
                    bossController.RecieveDamage(25);
                }
                else if (bossController2 != null)
                {
                    bossController2.RecieveDamage(25);

                }
                else if (tornadoEnemy)
                {
                    tornadoEnemy.RecieveDamage(100);
                }
                else
                {
                    LongDistanceEnemy enemyLongDistance = enemy.GetComponentInParent<LongDistanceEnemy>();

                    if (enemyLongDistance != null)
                    {
                        enemyLongDistance.RecieveDamage(100);
                    }

                }
            }
            isAttacking = false;
            player.isAttack = false;

        }

            
        // Puedes agregar más lógica de ataque aquí
        
    }

    // Método llamado por un evento de animación al inicio del ataque
    void StartAttack()
    {
        attackPS.Play();
        animacionEnCurso = true;
        tiempoInicioAnimacion = Time.time;
        isAttacking = true;
    }

    // Método llamado por un evento de animación al final del ataque
    void EndAttack()
    {
        Debug.Log("attaque end");
        isAttacking = false;
        player.isAttack = false;
        attackImage.sprite = attackDisabledSprite;

    }

    private void OnDrawGizmosSelected()
    {
        // Dibuja un gizmo esférico para visualizar el rango de ataque
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
