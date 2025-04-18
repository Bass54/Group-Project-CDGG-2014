using UnityEngine;
using UnityEngine.AI;
public class BaseEnemyMovement : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform[] waypoints;
    private int CurrentWayPoint;
    [SerializeField] private GameObject Player;
    [SerializeField] private float ChaseTimerMax;
    private float ChaseTimer;
    [SerializeField] private float WalkSpeed;
    [SerializeField] private float ChaseSpeed;
    [SerializeField] private LayerMask raycastLayerMask;
    private float raycastDistance = 100f;
    [SerializeField] private float TrapSpeed;
    [SerializeField] private float TrapTimerMax;
    private float TrapTimer = 0;
    private bool isTrapped = false;

    void Start() {
        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination (waypoints[0].position);
        agent.speed = WalkSpeed;
        ChaseTimer = 0f;
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
            }
            else {
                agent.speed = WalkSpeed;
            }
            if(agent.remainingDistance < agent.stoppingDistance)
            {
                CurrentWayPoint = (CurrentWayPoint + 1) % waypoints.Length;
                agent.SetDestination (waypoints[CurrentWayPoint].position);
            }
        }
        else {
            agent.SetDestination(Player.transform.position);
            if (isTrapped) {
                agent.speed = TrapSpeed;
            }
            else {
                agent.speed = ChaseSpeed;
            }
        }
        
    }

    void OnTriggerStay(Collider other) {
        if(other.gameObject == Player) {
            CheckLineOfSight();
        }

        if (other.gameObject.CompareTag("Trap")) {
            TrapTimer = TrapTimerMax;
        }
    }

    private void CheckLineOfSight() {
        Vector3 direction = Player.transform.position - transform.position;
        Ray ray = new Ray(transform.position + Vector3.up, direction.normalized);
        RaycastHit hit;


        if (Physics.Raycast(ray, out hit, raycastDistance, raycastLayerMask))
        {
            if (hit.collider.gameObject == Player)
            {
                ChaseTimer = ChaseTimerMax;
            }
        }
    }
}
