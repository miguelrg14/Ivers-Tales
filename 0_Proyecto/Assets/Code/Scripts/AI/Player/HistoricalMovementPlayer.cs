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
[DisallowMultipleComponent] //for not adding more than one
[RequireComponent(typeof(Player_controler))]
public class HistoricalMovementPlayer : MonoBehaviour
{
    private Player_controler player;
    [SerializeField]
    [Range(0.1f, 5f)]
    private float historicalPositionDuration=1f;
    [SerializeField]
    [Range(0.1f, 5f)]
    private float historicalPositionInterval = 0.1f;
    Vector3 item1;
    Vector3 item2;
    bool turnsForVectors = false;
    public Vector3 averageVelocity
    {
        get
        {
            Vector3 average = Vector3.zero;
            foreach (Vector3 velocity in historicalVelocities)
                average += velocity;
            average.y = 0;
            Vector3 result=average / historicalVelocities.Count;

            if (!double.IsNaN(result.x)|| !double.IsNaN(result.y)|| !double.IsNaN(result.z)) //checks if the result is not nan, because in the first frame it is due to the queue is not created
                return average / historicalVelocities.Count;
            else 
                return Vector3.zero; 
        }
    }
    private Queue<Vector3> historicalVelocities;
    private Queue<Vector3> historicalPositions;
    private float lasPositionTime;
    private int maxQueueSize;

    // Start is called before the first frame update
    void Awake()
    {
        player=GetComponent<Player_controler>();
        maxQueueSize=Mathf.CeilToInt(1f / historicalPositionInterval * historicalPositionDuration);
        historicalVelocities=new Queue<Vector3>(maxQueueSize);
        historicalPositions = new Queue<Vector3>(maxQueueSize);
    }

    // Update is called once per frame
    void Update()
    {
        if (lasPositionTime+historicalPositionInterval <=Time.time)
        {
            if (historicalVelocities.Count == maxQueueSize)
            {
                historicalVelocities.Dequeue();
                historicalPositions.Dequeue();
            }
            historicalVelocities.Enqueue(player.ReturnVelocityRigidbody());
            if (!turnsForVectors)
            {
                item1 = player.transform.position;
                turnsForVectors = true;
            }
            else
            {
                item2 = player.transform.position;
                turnsForVectors = false;

            }
            historicalPositions.Enqueue(player.transform.position);
            lasPositionTime = Time.time;
        }
    }
    public bool CompareTheLastVector3sPosition(float Epsilon = .1f)
    {
        Vector3 offset = item1 - item2;
        float sqrLen = offset.sqrMagnitude; //calculates distance with sqr magnitude instead of vector3.distance
        if (sqrLen < Epsilon)
            return false;
        else
            return true;
    }

        
    
}
