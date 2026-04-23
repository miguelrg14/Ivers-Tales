/* ================================================================
   ----------------------------------------------------------------
   Project   :   Iver Tales
   Publisher :   IguanaGo Studios
   Developer :   Lucas García Domínguez
   ----------------------------------------------------------------
   Copyright 2023-2024 IguanaGoStudios All rights reserved.
*/

using System.Drawing;
using TMPro;
using UnityEngine;
[System.Serializable]
public class PointForMeleeUnits
{
    public Vector3 position {private get;set;}
    public float distanceRadius { private get; set; }

    private bool isOcuppied=false;
    [SerializeField]
    private ShortDistanceEnemy enemyOcuppying;
    public PointForMeleeUnits(Vector3 pos,float distanceRadius)
    {
        this.position = pos;
        this.distanceRadius = distanceRadius;
        enemyOcuppying = null;
    }
    public bool ReturnIfEnemyHasReachedPoint()
    {
        Vector3 offset = enemyOcuppying.transform.position - position;
        float sqrLen = offset.sqrMagnitude; //calculates distance with sqr magnitude instead of vector3.distance
        if (sqrLen < distanceRadius+0.1f)   //with an offset for making it more generous
        { return true; }
        return false;
    }
    #region Getters And Setters Methods
    public void SetEnemy(ShortDistanceEnemy enemy)
    {
        enemyOcuppying = enemy;
        isOcuppied = true;
    }
    public void DeleteEnemy()
    {
        if (enemyOcuppying != null)
            enemyOcuppying = null;
        isOcuppied = false;

    }
    public bool IsOcuppied()
    {
        return isOcuppied;
    }
    public Vector3 ReturnPositionPoint()
    {
        return position;
    }
    #endregion
}
