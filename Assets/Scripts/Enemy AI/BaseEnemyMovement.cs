using UnityEngine;
using UnityEngine.AI;

public class BaseEnemyMovement : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private Transform[] FollowerPenguins;
    private int CurrentWayPoint;
    [SerializeField] private GameObject Player;
    [SerializeField] private float ChaseTimerMax = 15f;
    private float ChaseTimer;
    [SerializeField] private float WalkSpeed = 1f;
    [SerializeField] private float ChaseSpeed = 3f;
    [SerializeField] private LayerMask raycastLayerMask;
    private float raycastDistance = 100f;
    [SerializeField] private float TrapSpeed = 0.33f;
    [SerializeField] private float TrapTimerMax = 6f;
    private float TrapTimer = 0f;
    private bool isTrapped = false;

    private bool DetectPlayer;
    private bool[] DetectFollowers;
    private int ClosesDetectedFollower;

    private int AnimState; // 0 = walk, 1 = Chase, 2 = Trapped
    [SerializeField] private Animator anim;


    void Start() {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        agent.SetDestination (waypoints[0].position);
        agent.speed = WalkSpeed;
        AnimState = 0;
        ChaseTimer = 0f;

        DetectFollowers = new bool[FollowerPenguins.Length];
        for (int i = 0; i < DetectFollowers.Length; i++) {
            DetectFollowers[i] = false;
        }
    }

    void Update() {
        ChaseTimer -= Time.deltaTime;
        TrapTimer -= Time.deltaTime;

        if (TrapTimer >= 0) {
            isTrapped = true;
        }
        else {
            isTrapped = false;
        }

        if (ChaseTimer <= 0) {
            if (isTrapped) {
                agent.speed = TrapSpeed;
                AnimState = 2;
            }
            else {
                agent.speed = WalkSpeed;
                AnimState = 0;
            }
            if(agent.remainingDistance < agent.stoppingDistance)
            {
                CurrentWayPoint = (CurrentWayPoint + 1) % waypoints.Length;
                agent.SetDestination (waypoints[CurrentWayPoint].position);
            }

            for (int i = 0; i < FollowerPenguins.Length; i++) {
                DetectFollowers[i] = false;
            }
            DetectPlayer = false;
        }
        else {

            agent.isStopped = false;
            if (isTrapped) {
                agent.speed = TrapSpeed;
                AnimState = 2;
                Chase();
            }
            else {
                agent.speed = ChaseSpeed;
                AnimState = 1;
                Chase();
            }
        }
        anim.SetInteger("AnimState", AnimState);
    }

    void OnTriggerStay(Collider other) {

        if(other.gameObject == Player) {
            CheckLineOfSight(Player);
        }
        for (int i = 0; i < FollowerPenguins.Length; i++) {
            if (FollowerPenguins[i] != null) {
                if (other.gameObject == FollowerPenguins[i].gameObject) {
                    CheckLineOfSight(FollowerPenguins[i].gameObject);
                }
            }
        }

        if (other.gameObject.CompareTag("Trap"))
        {
            TrapTimer = TrapTimerMax;
        }
    }

    private void CheckLineOfSight(GameObject Object) {
        
        Vector3 origin = transform.position + Vector3.up * 1.5f;
        Vector3 targetPoint = Object.transform.position + Vector3.up * 0.5f;
        Vector3 direction = (targetPoint - origin).normalized;
        Ray ray = new Ray(origin, direction);
        RaycastHit hit;


        Debug.DrawRay(ray.origin, ray.direction * raycastDistance, Color.red, 1f);


        if (Physics.Raycast(ray, out hit, raycastDistance, raycastLayerMask))
        {
            if (hit.collider.gameObject == Object)
            {
                if (Object == Player) {
                    DetectPlayer = true;
                    ChaseTimer = ChaseTimerMax;
                }
                else {
                    for (int x = 0; x < FollowerPenguins.Length; x++) {
                        if (FollowerPenguins[x] != null) {
                            if (Object.name == FollowerPenguins[x].name) {
                                DetectFollowers[x] = true;
                                ChaseTimer = ChaseTimerMax;
                            }
                        }
                    }
                }
            }
        }
    }

    private void FindClosestDetected() {
        float DistanceToBeat = Mathf.Infinity;
        float Distance = 0;
        bool Detected = false;

        for (int i = 0; i < FollowerPenguins.Length; i++) {
            if (DetectFollowers[i]) {
                if (FollowerPenguins[i] != null) {
                    Distance = Vector3.Distance(transform.position, FollowerPenguins[i].position);

                    if (Distance <= DistanceToBeat) {
                        DistanceToBeat = Distance;
                        ClosesDetectedFollower = i;
                        Detected = true;
                    }
                }
            }
        }

        if (!Detected) {
            ClosesDetectedFollower = -1;
        }
    }

    private void Chase() {
        FindClosestDetected();
        if (ClosesDetectedFollower != -1) {
            agent.SetDestination(FollowerPenguins[ClosesDetectedFollower].position);
        }
        else if (DetectPlayer) {
            agent.SetDestination(Player.transform.position);
        }
    }
}