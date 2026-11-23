/* ================================================================
   ----------------------------------------------------------------
   Project   :   Iver Tales
   Publisher :   IguanaGo Studios
   Developer :   Lucas García Domínguez
   ----------------------------------------------------------------
   Copyright 2023-2024 IguanaGoStudios All rights reserved.
*/

using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    [SerializeField]
    Transform m_Target;
    [SerializeField]
    float updateSpeed = 0.15f;
    private NavMeshAgent agent;
    private PlayerController controllerPlayer;
    [Header("Movement Prediction")]
    [SerializeField]
    private bool useMovementPrediction;
    [SerializeField]
    [Range(-1,1)]
    private float movementPredictionThreshold = 0;
    [SerializeField]
    [Range(0.25f, 2f)]
    private float movementPredictionTime = 1f;
    public GameObject[] enemies;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (useMovementPrediction)
        {

            controllerPlayer = m_Target.GetComponent<PlayerController>();
        }

    }

    void Start()
    {
        StartCoroutine(FollowTarget());
    }

    void Update()
    {
        // Get the list of enemies
         enemies = GameObject.FindGameObjectsWithTag("EnemyForTest");

        // Size of each segment of the circle
        float segmentSize = 360f / enemies.Length;

        for (int i = 0; i < enemies.Length; i++)
        {
            GameObject enemy = enemies[i];

            // Direction in which the enemy is chasing
            Vector3 direction = (controllerPlayer.transform.position - enemy.transform.position).normalized;

            // Get the list of points on the circle
            Vector3[] points = GetPointsInCircle(controllerPlayer.transform.position, 2, enemies.Length);

            // Angle of the circle segment assigned to this enemy
            float minAngle = i * segmentSize;
            float maxAngle = (i + 1) * segmentSize;

            // Keep the points inside this enemy's segment
            Vector3[] segmentPoints = points.Where(point => IsPointInSegment(point, controllerPlayer.transform.position, minAngle, maxAngle)).ToArray();

            // Closest point in the chasing direction
            Vector3 closestPoint = GetClosestPointInDirection(segmentPoints, enemy.transform.position, direction);

            // Move the enemy towards that point
            enemy.GetComponent<NavMeshAgent>().SetDestination(closestPoint);
        }
    }
    Vector3[] GetPointsInCircle(Vector3 center, float radius, int numPoints)
    {
        Vector3[] points = new Vector3[numPoints];

        for (int i = 0; i < numPoints; i++)
        {
            float angle = i * Mathf.PI * 2f / numPoints;
            points[i] = center + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
        }

        return points;
    }
    Vector3 GetClosestPointInDirection(Vector3[] points, Vector3 position, Vector3 direction)
    {
        Vector3 closestPoint = Vector3.zero;
        float closestDistance = Mathf.Infinity;

        foreach (Vector3 point in points)
        {
            Vector3 toPoint = point - position;
            float distance = toPoint.magnitude;

            // Check whether the point is in the chasing direction
            if (Vector3.Dot(toPoint.normalized, direction) > 0 && distance < closestDistance)
            {
                closestPoint = point;
                closestDistance = distance;
            }
        }

        return closestPoint;
    }
    bool IsPointInSegment(Vector3 point, Vector3 center, float minAngle, float maxAngle)
    {
        Vector3 toPoint = (point - center).normalized;
        float angle = Mathf.Atan2(toPoint.z, toPoint.x) * Mathf.Rad2Deg;

        return angle >= minAngle && angle <= maxAngle;
    }
    private IEnumerator FollowTarget()
    {
        WaitForSeconds wait= new WaitForSeconds(updateSpeed);
        while (enabled)
        {
            if (useMovementPrediction)
            {
                agent.SetDestination(m_Target.transform.position+ controllerPlayer.ReturnAverageVelocity()*movementPredictionTime);

            }
            else
            {
                agent.SetDestination(m_Target.position);

            }
            yield return wait;

        }
    }
}
