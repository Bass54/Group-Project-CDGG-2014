using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerFollower : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform followTarget;
    private static List<Transform> followers = new List<Transform>();
    private static int count = 0;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (followTarget != null)
        {
            agent.SetDestination(followTarget.position);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && followTarget == null) // Ensures it only triggers once
        {
            if (followers.Count == 0)
            {
                followTarget = other.transform;
            }
            else
            {
                followTarget = followers[followers.Count - 1];
            }

            followers.Add(transform);
            count++;
            Debug.Log("Follower Count: " + count);
        }
    }
}

