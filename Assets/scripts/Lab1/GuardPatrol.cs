using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using NUnit.Framework;

public class GuardPatrol : MonoBehaviour
{

    NavMeshAgent nva;

    public Transform player;
    public List<Transform> waypoints = new List<Transform>(4);
    int currentwaypoint = 0;

    public enum GuardState
    {
        Patrolling,
        Chasing,
        Returning
    }

    public GuardState curState = GuardState.Patrolling;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nva = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (curState)
        {
            case GuardState.Patrolling:
                if(!nva.pathPending && nva.remainingDistance < 1f) {
                    nva.SetDestination(waypoints[currentwaypoint].position);
                    currentwaypoint = (currentwaypoint + 1) % waypoints.Count;
                }
                break;
            case GuardState.Chasing:
                
                nva.SetDestination(player.position);
                break;
            case GuardState.Returning:
                if (nva.remainingDistance < 1f)
                {
                    Debug.Log("Returned to patrol.");
                    curState = GuardState.Patrolling;
                }
                break;
        }
        
    }


     private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player detected!");
            curState = GuardState.Chasing;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player lost!");
            curState = GuardState.Returning;
            nva.SetDestination(waypoints[currentwaypoint].position);
        }
    }
}