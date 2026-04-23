using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    [SerializeField]
    PoolingOfVFX poolVFXEnemyDie;

    public delegate void VFXEventHandlerEnemyDie(Vector3 Player, Quaternion rotation);    //their firms

    public event VFXEventHandlerEnemyDie OnEnableParticleVFXEnemyDie;    //events for when the player enter and exits

    [SerializeField]
    PoolingOfVFX poolVFXBulletsDestroy;

    public delegate void VFXEventHandlerBulletInGround(Vector3 Player, Quaternion rotation);    //their firms

    public event VFXEventHandlerBulletInGround OnEnableParticleVFXBullet;    //events for when the player enter and exits

    [SerializeField]
    PoolingOfVFX poolVFXRecieveDamagePlayer;

    public delegate void VFXEventHandlerRecieveDamagePlayer(Vector3 Player, Quaternion rotation);    //their firms

    public event VFXEventHandlerRecieveDamagePlayer OnEnableParticleVFXPlayerDamage;    //events for when the player enter and exits

    [SerializeField]
    PoolingOfVFX poolVFXRecieveDamagePlayerFromEnemyTornado;

    public delegate void VFXEventHandlerRecieveDamagePlayerFromEnemyTornado(Vector3 Player, Quaternion rotation);    //their firms

    public event VFXEventHandlerRecieveDamagePlayer OnEnableParticleVFXPlayerFromEnemyTornado;    //events for when the player enter and exits

    [SerializeField]
    PoolingOfVFX poolVFXWallsDissapearBossL2;

    public delegate void VFXEventHandlerDissapearWallsBossL2(Vector3 Player, Quaternion rotation);    //their firms

    public event VFXEventHandlerDissapearWallsBossL2 OnEnableParticlepoolVFXWallsDissapearBossL2;    //events for when the player enter and exits
    // Start is called before the first frame update
    void Start()
    {
        OnEnableParticleVFXEnemyDie += EnableParticleVFXEnemyDie;
        OnEnableParticleVFXBullet += EnableParticleVFXBullet;
        OnEnableParticleVFXPlayerDamage += EnableParticleVFXPlayerRecieveDamage;
        OnEnableParticleVFXPlayerFromEnemyTornado += EnableParticleVFXPlayerRecieveDamageFromEnemyTornado;
        OnEnableParticlepoolVFXWallsDissapearBossL2 += EnableParticleVFXWallsDissapearBossL2;

    }
    void EnableParticleVFXEnemyDie(Vector3 enemy,Quaternion rotation)
    {
        poolVFXEnemyDie.EnableParticleSystem(enemy, rotation);
    }
    public void ActivateVFXEnemyDie(Vector3 enemy, Quaternion rotation)
    {
        OnEnableParticleVFXEnemyDie?.Invoke(enemy, rotation);
    }
    void EnableParticleVFXBullet(Vector3 positionBulletCollision, Quaternion rotation)
    {
        poolVFXBulletsDestroy.EnableParticleSystem(positionBulletCollision, rotation);
    }
    public void ActivateVFXBullet(Vector3 positionBulletCollision, Quaternion rotation)
    {
        OnEnableParticleVFXBullet?.Invoke(positionBulletCollision, rotation);
    }
    void EnableParticleVFXPlayerRecieveDamage(Vector3 enemy, Quaternion rotation)
    {
        poolVFXRecieveDamagePlayer.EnableParticleSystem(enemy,rotation);
    }
    public void ActivateVFXPlayerRecieveDamage(Vector3 enemy, Quaternion rotation)
    {
        OnEnableParticleVFXPlayerDamage?.Invoke(enemy, rotation);
    }
    void EnableParticleVFXPlayerRecieveDamageFromEnemyTornado(Vector3 enemy, Quaternion rotation)
    {
        poolVFXRecieveDamagePlayerFromEnemyTornado.EnableParticleSystem(enemy, rotation);
    }
    public void ActivateVFXPlayerRecieveDamageFromEnemyTornado(Vector3 enemy, Quaternion rotation)
    {
        OnEnableParticleVFXPlayerFromEnemyTornado?.Invoke(enemy, rotation);
    }
    void EnableParticleVFXWallsDissapearBossL2(Vector3 enemy, Quaternion rotation)
    {
        poolVFXWallsDissapearBossL2.EnableParticleSystem(enemy, rotation);
    }
    public void ActivateVFXWallsDissapearBossL2(Vector3 enemy, Quaternion rotation)
    {
        OnEnableParticlepoolVFXWallsDissapearBossL2?.Invoke(enemy, rotation);
    }
}
