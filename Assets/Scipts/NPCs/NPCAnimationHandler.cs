using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCAnimationHandler : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent agent;
    private EnermyController ec;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        if (GetComponent<EnermyController>() != null )
        {
            ec = GetComponent<EnermyController>();   
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 vel = Vector3.zero;
        if (agent != null) vel = agent.velocity;

        animator.SetFloat("vel", vel.magnitude);

        if (ec != null)
        {
            EnermyController.State state = ec.state;
            if (vel.magnitude == 0 && state == EnermyController.State.LOCKEDON)
            {
                animator.SetBool("aiming", true);
            }
            else
            {
                animator.SetBool("aiming", false);
            }
        }
    }
}
