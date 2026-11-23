using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

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
    [FormerlySerializedAs("poolVFXRecieveDamagePlayer")]
    PoolingOfVFX poolVFXReceiveDamagePlayer;

    public delegate void VFXEventHandlerReceiveDamagePlayer(Vector3 Player, Quaternion rotation);    //their firms

    public event VFXEventHandlerReceiveDamagePlayer OnEnableParticleVFXPlayerDamage;    //events for when the player enter and exits

    [SerializeField]
    [FormerlySerializedAs("poolVFXRecieveDamagePlayerFromEnemyTornado")]
    PoolingOfVFX poolVFXReceiveDamagePlayerFromEnemyTornado;

    public delegate void VFXEventHandlerReceiveDamagePlayerFromEnemyTornado(Vector3 Player, Quaternion rotation);    //their firms

    public event VFXEventHandlerReceiveDamagePlayer OnEnableParticleVFXPlayerFromEnemyTornado;    //events for when the player enter and exits

    [SerializeField]
    [FormerlySerializedAs("poolVFXWallsDissapearBossL2")]
    PoolingOfVFX poolVFXWallsDisappearBossL2;

    public delegate void VFXEventHandlerDisappearWallsBossL2(Vector3 Player, Quaternion rotation);    //their firms

    public event VFXEventHandlerDisappearWallsBossL2 OnEnableParticlepoolVFXWallsDisappearBossL2;    //events for when the player enter and exits

    void Start()
    {
        OnEnableParticleVFXEnemyDie += EnableParticleVFXEnemyDie;
        OnEnableParticleVFXBullet += EnableParticleVFXBullet;
        OnEnableParticleVFXPlayerDamage += EnableParticleVFXPlayerReceiveDamage;
        OnEnableParticleVFXPlayerFromEnemyTornado += EnableParticleVFXPlayerReceiveDamageFromEnemyTornado;
        OnEnableParticlepoolVFXWallsDisappearBossL2 += EnableParticleVFXWallsDisappearBossL2;

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
    void EnableParticleVFXPlayerReceiveDamage(Vector3 enemy, Quaternion rotation)
    {
        poolVFXReceiveDamagePlayer.EnableParticleSystem(enemy,rotation);
    }
    public void ActivateVFXPlayerReceiveDamage(Vector3 enemy, Quaternion rotation)
    {
        OnEnableParticleVFXPlayerDamage?.Invoke(enemy, rotation);
    }
    void EnableParticleVFXPlayerReceiveDamageFromEnemyTornado(Vector3 enemy, Quaternion rotation)
    {
        poolVFXReceiveDamagePlayerFromEnemyTornado.EnableParticleSystem(enemy, rotation);
    }
    public void ActivateVFXPlayerReceiveDamageFromEnemyTornado(Vector3 enemy, Quaternion rotation)
    {
        OnEnableParticleVFXPlayerFromEnemyTornado?.Invoke(enemy, rotation);
    }
    void EnableParticleVFXWallsDisappearBossL2(Vector3 enemy, Quaternion rotation)
    {
        poolVFXWallsDisappearBossL2.EnableParticleSystem(enemy, rotation);
    }
    public void ActivateVFXWallsDisappearBossL2(Vector3 enemy, Quaternion rotation)
    {
        OnEnableParticlepoolVFXWallsDisappearBossL2?.Invoke(enemy, rotation);
    }
}
