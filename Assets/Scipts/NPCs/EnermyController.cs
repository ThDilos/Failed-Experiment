using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnermyController : MonoBehaviour
{
    public enum State
    {
        // Stay / Wandering ->
        // Alerted ->
        // Scouting
        // During the above 4 states, if see player -> LockedOn
        STAY, // Not Moving, Nor Reacting at all
        WANDERING, // Randomly walking around // Set range to 0 to not move at all
        ALERTED, // Noticed something, going to check
        SCOUTING, // May follow a presetted path or not
        LOCKEDON, // Discovered target, moving towards it
        FIRING, // Shooting at Target
    }


    public void SetState(State val)
    {
        if (agent.pathEndPosition != transform.position)
        {
            agent.ResetPath();
        }
        state = val;
        SetStats(val);
        currentTime = 0;
    }

    public NavMeshAgent agent;
    private FieldOfView fov;

    [SerializeField] private Shoot gun;

    [HideInInspector] public Vector3 playerPos; // Player's realtime position
    [HideInInspector] public Player player; // Player self
    private Vector3 targetPos; // Position to move into
    private Vector3 spotToCheck; // Used in Alert mode

    [SerializeField] public State state = State.STAY; // Default state of this NPC

    [SerializeField] private Transform headTransform;
    [SerializeField] private Transform bodyTransform;

    [SerializeField]
    private float rotationSpeed = 0.1f; // How fast the enermy can turn around to look

    [Header("Wandering")]
    [SerializeField] private float wanderingVisionRange = 25.0f;
    [SerializeField][Range(0, 360)] private float wanderingVisionAngle = 60.0f;
    [SerializeField] private float wanderingSpeed = 10.0f;
    [SerializeField] private float timeUntilNextMove = 5.0f; // Time paused after reaching a destination
    [SerializeField] private float rangeOfWandering = 5.0f; // Range from wandering center that the entity can go to
    private Vector3 wanderingCenter; // Center of wandering, default is the scene placement position

    [Header("Alerted")]
    [SerializeField] private float alertedVisionRange = 50.0f;
    [SerializeField][Range(0, 360)] private float alertedVisionAngle = 100.0f;

    [Header("SCOUTING")]
    [SerializeField] private float scoutingSpeed = 8.0f;

    [Header("LockedOn")]
    [SerializeField] private float lockedOnVisionRange = 80.0f;
    [SerializeField][Range(0, 360)] private float lockedOnVisionAngle = 80.0f;
    [SerializeField] private float lockedOnSpeed = 15.0f;
    [SerializeField] private float timeToLoseTarget = 5.0f;


    // Runtime variables
    private float currentTime = 0.0f;
    private State previousState;

    void Start()
    {
        wanderingCenter = transform.position;
        agent = GetComponent<NavMeshAgent>();
        fov = GetComponent<FieldOfView>();
        previousState = state;
        SetState(state);
    }

    // Update is called once per frame
    void Update()
    {
        if (agent != null)
        {
            if (fov.canSeePlayer)
            {
                playerPos = fov.playerCenter;
                player = fov.player;
            }
            switch (state)
            {
                case State.STAY:
                    break;

                // Wandering State, move to a random position within the circle, centered at wanderingCenter, radius of rangeOfWandering, and repeat the action every timeUntilNextMove
                case State.WANDERING:
                    if (agent.velocity.magnitude == 0)
                    {
                        currentTime -= Time.deltaTime;
                    }
                    if (currentTime < 0)
                    {
                        currentTime = timeUntilNextMove;
                        targetPos = new Vector3(wanderingCenter.x + Random.Range(-rangeOfWandering, rangeOfWandering), wanderingCenter.y + Random.Range(-rangeOfWandering, rangeOfWandering), wanderingCenter.z + Random.Range(-rangeOfWandering, rangeOfWandering));
                    }
                    break;

                case State.ALERTED:
                    Debug.Log("Alerted");
                    if (spotToCheck != null)
                    {
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(spotToCheck), rotationSpeed * Time.deltaTime);
                    }
                    break;

                case State.LOCKEDON:
                    if (playerPos != null)
                    {
                        targetPos = playerPos;
                        currentTime = timeToLoseTarget;
                        //headTransform.rotation = Quaternion.RotateTowards(headTransform.rotation, Quaternion.Euler(playerPos), rotationSpeed * Time.deltaTime);
                        Debug.DrawRay(headTransform.position, headTransform.forward, Color.yellow);
                    }
                    if (agent.velocity.magnitude == 0)
                    {
                        currentTime -= Time.deltaTime;
                    }
                    if (currentTime < 0)
                    {
                        SetState(previousState);
                        Debug.Log("State changed into " + previousState);
                    }
                    if (player != null && (player.transform.position - transform.position).magnitude < agent.stoppingDistance)
                    {
                        agent.SetDestination(transform.position);
                        SetState(State.FIRING);
                    }
                    if (player != null && (player.transform.position - transform.position).magnitude < 2.0f)
                    {
                        player.Die();
                        Destroy(agent);
                    }
                    break;
                case State.FIRING:
                    if (playerPos != null)
                    {
                        targetPos = playerPos;
                        Debug.DrawRay(headTransform.position, headTransform.forward, Color.yellow);
                    }
                    if (player != null && (player.transform.position - transform.position).magnitude > agent.stoppingDistance * 2)
                    {
                        Debug.Log("Go Back to locked on");
                        if (gun != null) gun.firing = false;
                        SetState(State.LOCKEDON);
                    }
                    else if (gun != null && !player.died) 
                    {
                        gun.target = player.transform;
                        gun.firing = true;
                    }
                    if (player.died)
                    {
                        gun.firing = false;
                    }
                    break;

            }
            if (targetPos != null && transform.position != targetPos && state != State.ALERTED)
            {
                if (state != State.FIRING && !(player != null && ((player.transform.position - transform.position).magnitude < agent.stoppingDistance)))
                {
                    if ((Vector3.Distance(transform.position, targetPos) > agent.stoppingDistance / 5) || agent.isStopped)
                        agent.SetDestination(targetPos);
                }

                Vector3 targetRot = targetPos - transform.position;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(new Vector3(targetRot.x, 0, targetRot.z)), rotationSpeed / 100);
            }

            // Agent Speed Limit
            if (agent.velocity.magnitude > getSpeedLimit(state)) 
                agent.velocity = Vector3.ClampMagnitude(agent.velocity, getSpeedLimit(state));
        }
    }

    public void Alert(Vector3 sourcePos)
    {
        if (state != State.LOCKEDON)
        {
            SetState(State.ALERTED);
            spotToCheck = sourcePos;
        }
    }

    public void SetStats(State state)
    {
        Debug.Log("State changed to " + state);
        this.state = state;
        switch (state)
        {
            case State.STAY:
                break;
            case State.WANDERING:
                fov.radius = wanderingVisionRange;
                fov.angle = wanderingVisionAngle;
                agent.speed = wanderingSpeed;
                break;
            case State.ALERTED:
                fov.radius = alertedVisionRange;
                fov.angle = alertedVisionAngle;
                break;
            case State.SCOUTING:
                agent.speed = scoutingSpeed;
                break;
            case State.LOCKEDON:
                fov.radius = lockedOnVisionRange;
                fov.angle = lockedOnVisionAngle;
                agent.speed = lockedOnSpeed;
                break;
            case State.FIRING:
                break;
        }
    }

    private float getSpeedLimit(State state)
    {
        switch (state)
        {
            case State.LOCKEDON: return lockedOnSpeed;
            case State.SCOUTING: return scoutingSpeed;
            case State.WANDERING: return wanderingSpeed;
        }
        return wanderingSpeed;
    }
}
