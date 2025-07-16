using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

// Original code by Comp-3 Interactive
// https://www.youtube.com/watch?v=j1-OyLo77ss

public class FieldOfView : MonoBehaviour
{
    [HideInInspector] public float radius;
    [HideInInspector] [Range(0, 360)] public float angle;

    [HideInInspector] public GameObject playerRef;
    [HideInInspector] public Vector3 playerCenter;
    [HideInInspector] public Player player;

    public LayerMask targetMask;
    public LayerMask obstructionMask;

    [SerializeField] private Vector3 visionOffset = Vector3.zero;

    [HideInInspector] public bool canSeePlayer;

    private EnermyController enermyController;

    private void Start()
    {
        enermyController = GetComponent<EnermyController>();
        playerRef = GameObject.FindGameObjectWithTag("Player");
        player = playerRef.GetComponent<Player>();
        StartCoroutine(FOVRoutine()); // Put this in a delay for performance
    }

    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position + visionOffset, radius, targetMask);
        playerCenter = playerRef.transform.Find("fox").position + playerRef.GetComponent<CapsuleCollider>().center;
        if (rangeChecks.Length != 0) 
        { 
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (playerCenter - transform.position + visionOffset).normalized;

            // Check if player is in the setted vision angle
            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position + visionOffset, playerCenter);

                // If can see player and not an obstruction, canSeePlayer = true
                if (!Physics.Raycast(transform.position + visionOffset, directionToTarget, distanceToTarget, obstructionMask)) canSeePlayer = true; 
                else canSeePlayer = false;
            }
            else canSeePlayer = false;
        }
        else if (canSeePlayer) canSeePlayer = false; // Player isn't within range of player anymore
        if (canSeePlayer && enermyController.state != EnermyController.State.LOCKEDON && enermyController.state != EnermyController.State.FIRING)
        {
            enermyController.SetState(EnermyController.State.LOCKEDON);
        }
    }

    // Can see during runTime
    void OnDrawGizmos()
    {
        Vector3 viewAngle01 = DirectionFromAngle(transform.eulerAngles.y, -angle / 2);
        Vector3 viewAngle02 = DirectionFromAngle(transform.eulerAngles.y, angle / 2);

        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position + visionOffset, transform.position + viewAngle01 * radius);
        Gizmos.DrawLine(transform.position + visionOffset, transform.position + viewAngle02 * radius);

        if (canSeePlayer)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position + visionOffset, playerCenter);
        }
    }

    private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
