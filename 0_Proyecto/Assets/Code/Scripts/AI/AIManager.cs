/* ================================================================
   ----------------------------------------------------------------
   Project   :   Iver Tales
   Publisher :   IguanaGo Studios
   Developer :   Lucas García Domínguez
   ----------------------------------------------------------------
   Copyright 2023-2024 IguanaGoStudios All rights reserved.
*/

using System.Collections.Generic;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

[DefaultExecutionOrder(0)]
public class AIManager : MonoBehaviour
{
    private static AIManager _instance;
    public static AIManager Instance
    {
        get
        {
            return _instance;
        }
        private set
        {
            _instance = value;
        }
    }
    [SerializeField]
    public Transform target;
    [SerializeField]
    float radiusAroundTarget = 0.5f;
    [SerializeField]
    float distanceBetweenEnemies = 3;
    [SerializeField]
    public List<ShortDistanceEnemy> unitsInMeleeRange {get;private set;}
    public PointForMeleeUnits[] positionsForEnemies;
    [SerializeField]
    private int maximumNumberOfEnemiesForDungeon = 6;
    private float angleIncrement;   //the angle increment that is calculated only once

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            return;
        }

        Destroy(gameObject);
    }
    private void Start()
    {
        positionsForEnemies = new PointForMeleeUnits[maximumNumberOfEnemiesForDungeon];
        unitsInMeleeRange = new List<ShortDistanceEnemy>();
        InitializeValues(); //calculates the angle needed for making the circle only once, then it is multiplicated for the actual enemy int
        for (int i = 0; i < positionsForEnemies.Length; i++)
        {
            positionsForEnemies[i] = new PointForMeleeUnits(SetPositionsAroundMeleeCircle(i),radiusAroundTarget);  //sets the positions for the first time and initializes
        }
        CalculatePositionsForEnemies();

    }
    // Update is called once per frame
    void Update()
    {
        if (unitsInMeleeRange.Count>1)  //if its more or equal than 2, the update is not evaluating and the agents are not circling the target
        {
            MakeAgentsCircleTarget();
        }
        else if(unitsInMeleeRange.Count == 1)   //for checking if there is one unit 
        {
            if (unitsInMeleeRange[0] != null)
            {
                unitsInMeleeRange[0].SetOneEnemyMode(true);

            }
        }

    }
    #region Melee Squad System Positions
    public void MakeAgentsCircleTarget()  
    {
        CalculatePositionsForEnemies();
        PointForMeleeUnits pointClosest = new PointForMeleeUnits(Vector3.zero,radiusAroundTarget);
        float closestDistance;
        Vector3 direction;
        foreach (var unit in unitsInMeleeRange)
        {
            unit.SetOneEnemyMode(false); //sets the variable of going alone to false for having the enemy not following the posiitions of the points
            direction = (target.position - unit.transform.position).normalized;  //follows the player with the rotation
            unit.transform.rotation = Quaternion.LookRotation(direction);   //TODO make it look better via animations maybe
            if (unit.pointMelee==null)
            {
                pointClosest.position = Vector3.zero;   //sets the point closest to one that is zero
                closestDistance = 100; // Distance to the nearest point
                foreach (PointForMeleeUnits point in positionsForEnemies)
                {
                    if (point.IsOcuppied()) continue; // if the point is ocuppated, go to the next one

                    Vector3 offset = unit.transform.position - point.ReturnPositionPoint();
                    float sqrLen = offset.sqrMagnitude; //calculates distance with sqr magnitude instead of vector3.distance
                    if ((sqrLen < closestDistance))
                    {
                        closestDistance = sqrLen;
                        pointClosest = point;
                    }
                }
                unit.SetPositionMeleePoint(pointClosest);
            }

        }
    }
    public Vector3 ReturnNearestPoint(Vector3 positionEnemy)
    {
        CalculatePositionsForEnemies();
        PointForMeleeUnits pointClosest = new PointForMeleeUnits(Vector3.zero, radiusAroundTarget);
        float closestDistance;

        pointClosest.position = Vector3.zero;   //sets the point closest to one that is zero
        closestDistance = Mathf.Infinity; // Distance to the nearest point
        foreach (PointForMeleeUnits point in positionsForEnemies)
        {
            //if (point.IsOcuppied()) continue; // if the point is ocuppated, go to the next one

            Vector3 offset = point.ReturnPositionPoint() - positionEnemy;
            float sqrLen = offset.sqrMagnitude; //calculates distance with sqr magnitude instead of vector3.distance
            if ((sqrLen < closestDistance))
            {
                closestDistance = sqrLen;
                pointClosest = point;
            }
        }
        return pointClosest.ReturnPositionPoint();
    }
    void CalculatePositionsForEnemies()
    {
        for (int i = 0; i < positionsForEnemies.Length; i++)
        {
            positionsForEnemies[i].position = SetPositionsAroundMeleeCircle(i); //calculates the positions for the points based on the maximum amount of enemies posible
        }
    }
    Vector3 SetPositionsAroundMeleeCircle(int i)
    {
        return new Vector3(
                target.position.x + radiusAroundTarget * Mathf.Cos(angleIncrement * i),
                target.position.y,
                target.position.z + radiusAroundTarget * Mathf.Sin(angleIncrement * i)
                    );  //calculates the positions around the player in real time
    }
    public void InitializeValues()
    {
        angleIncrement = 2 * Mathf.PI / maximumNumberOfEnemiesForDungeon;
    }
    #endregion
    #region Old System for Perfect Circles
    //public void MakeAgentsCircleTarget()    //this would be best if the directions arent shortdistance call AIManager and sets a value in shortdistanceenmy
    //                                        //too much calls. It would be best if when shortdistancecalls aimanager it returns
    //                                        //a value and sets it inside their script
    //{

    //    if (unitsInMeleeRange.Count > 1)
    //    {
    //        for (int i = 0; i < unitsInMeleeRange.Count; i++)
    //        {

    //            unitsInMeleeRange[i].SetCircleTargetPosition(new Vector3(
    //                target.position.x + radiusAroundTarget * Mathf.Cos(2 * Mathf.PI * i / unitsInMeleeRange.Count),
    //                target.position.y,
    //                target.position.z + radiusAroundTarget * Mathf.Sin(2 * Mathf.PI * i / unitsInMeleeRange.Count)
    //                ));
    //            //obtains the exact position of the ai unity based on the number of units chasing the player + the radius
    //        }
    //    }
    //    else if (unitsInMeleeRange.Count == 1)
    //    {
    //        Debug.Log("Only One");
    //        unitsInMeleeRange[0].SetCircleTargetPosition(unitsInMeleeRange[0].ReturnActualTargetPosition());
    //        //returns the actual target position that is used in movement prediction beca¡use there is no more enemies
    //        //in the list, so there is no need tp make a circle
    //    }
    //}
    #endregion
    #region Gizmos
    private void OnDrawGizmos()
    {
        for (int i = 0; i < maximumNumberOfEnemiesForDungeon; i++)
        {
            Gizmos.DrawSphere(new Vector3(
                    target.position.x + radiusAroundTarget * Mathf.Cos(2 * Mathf.PI * i / maximumNumberOfEnemiesForDungeon),
                    target.position.y,
                    target.position.z + radiusAroundTarget * Mathf.Sin(2 * Mathf.PI * i / maximumNumberOfEnemiesForDungeon)
                    ), 0.25f);
        }
    }
    #endregion

}
