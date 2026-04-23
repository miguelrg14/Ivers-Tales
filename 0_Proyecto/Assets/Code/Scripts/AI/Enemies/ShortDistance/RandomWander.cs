using RenownedGames.AITree;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI; //important
using Color = UnityEngine.Color;
//if you use this code you are contractually obligated to like the YT video
public class RandomWander : MonoBehaviour //don't forget to change the script name if you haven't
{
    NavMeshAgent agent;
    public float range=10; //radius of sphere
    Transform centrePoint; //centre of the area the agent wants to move around in
    //instead of centrePoint you can set it as the transform of the agent if you don't care about a specific area
    [SerializeField]
    Vector3 positionTarget;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        centrePoint=agent.transform;
    }
    public void ReCalculatePosition()
    {
        StartCoroutine(ActivateDesactivateAgent());
    }
    private IEnumerator ActivateDesactivateAgent()  //this is the only way i found for starting wandering again when the enemy detects the player and starts chasing him
    {
        // Desactivate NavMeshAgent
        BehaviourRunner behRunner = agent.GetComponent<BehaviourRunner>();
        behRunner.enabled = false;
        agent.enabled = false;


        // Wait seconds before the agent gets desactivated
        yield return new WaitForSeconds(0.01f);

        // Desactivate  the NavMeshAgent
        behRunner.enabled = true;
        agent.enabled = true;

        StopCoroutine(ActivateDesactivateAgent());  
    }
    public void MoveToPosition()
    {
        if (agent.remainingDistance <= agent.stoppingDistance) //done with path
        {
            //Debug.Log(agent.destination);
            Vector3 point;
            if (RandomPoint(centrePoint.position, range, out point)) //pass in our centre point and radius of area
            {
                //Debug.DrawRay(point, Vector3.up, Color.blue, 4.0f); //so you can see with gizmos
                //positionTarget=transform.InverseTransformPoint(point);
                positionTarget =point;
                agent.SetDestination(point);
            }
        }
    }
    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        Vector3 randomPoint = center + Random.insideUnitSphere * range; //random point in a sphere 
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas)) //documentation: https://docs.unity3d.com/ScriptReference/AI.NavMesh.SamplePosition.html
        {
            //the 1.0f is the max distance from the random point to a point on the navmesh, might want to increase if range is big
            //or add a for loop like in the documentation
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }


}