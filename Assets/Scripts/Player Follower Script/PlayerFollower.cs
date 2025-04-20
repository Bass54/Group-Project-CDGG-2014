using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerFollower : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform followTarget;
    private static List<Transform> followers = new List<Transform>();
    public static int count = 0;
    private AudioSource encounterSound;

    void Start()
    {
        encounterSound = GetComponent<AudioSource>();
        agent = GetComponent<NavMeshAgent>();
     
    }

    void Update()
    {
        if (followTarget != null)
        {
            agent.SetDestination(followTarget.position);
        }
    }
    void playSound()
    {
        if (encounterSound != null && !encounterSound.isPlaying)
        {
            encounterSound.Play();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && followTarget == null) // Ensures it only triggers once
        {
            if (followers.Count == 0)
            {
                playSound();
                followTarget = other.transform;
            }
            else
            {
                playSound();
                followTarget = followers[followers.Count - 1];
            }

            followers.Add(transform);
            count++;
            Debug.Log("Follower Count: " + count);
        }
    }
    public static void DeleteFollowers()
    {
        foreach (Transform follower in followers)
        {
            if (follower != null)
            {
                Destroy(follower.gameObject);
            }
        }

        followers.Clear();
        count = 0;
    }
}

