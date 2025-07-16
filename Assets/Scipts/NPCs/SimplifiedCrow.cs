using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplifiedCrow : MonoBehaviour, IInteractable
{

    // Customed CrowNPC, modified from original 
    enum birdBehaviors
    {
        sing,
        preen,
        ruffle,
        peck
    }

    public AudioClip song1;
    public AudioClip song2;
    public AudioClip caw;

    public bool fleeCrows = true;

    Animator anim;

    bool paused = false;
    bool idle = true;
    bool onGround = true;
    bool dead = false;
    float agitationLevel = .5f;

    // Where to place the camera when interacting?
    public Transform camDestTransform;

    // Phase of Interaction
    [HideInInspector] public int interactionPhase = 0;

    //hash variables for the animation states and animation properties
    int idleAnimationHash;
    int singAnimationHash;
    int ruffleAnimationHash;
    int preenAnimationHash;
    int peckAnimationHash;
    int worriedAnimationHash;
    int peckBoolHash;

    int ruffleBoolHash;
    int preenBoolHash;
    int singTriggerHash;
    int dieTriggerHash;

    void OnEnable()
    {
        anim = gameObject.GetComponent<Animator>();
        idleAnimationHash = Animator.StringToHash("Base Layer.Idle");
        peckBoolHash = Animator.StringToHash("peck");
        ruffleBoolHash = Animator.StringToHash("ruffle");
        preenBoolHash = Animator.StringToHash("preen");
        singTriggerHash = Animator.StringToHash("sing");
        dieTriggerHash = Animator.StringToHash("die");
        worriedAnimationHash = Animator.StringToHash("worried");
        anim.SetFloat("IdleAgitated", agitationLevel);
    }

    void OnGroundBehaviors()
    {
        idle = anim.GetCurrentAnimatorStateInfo(0).fullPathHash == idleAnimationHash;
        //idle = anim.GetCurrentAnimatorStateInfo(0).nameHash == idleAnimationHash;
        if (idle)
        {
            //the bird is in the idle animation, lets randomly choose a behavior every 3 seconds
            if (Random.value < Time.deltaTime * .33)
            {
                //bird will display a behavior
                //in the perched state the bird can only sing, preen, or ruffle
                float rand = Random.value;
                if (rand < 0.5)
                {
                    Vector3 playerPos = GameObject.Find("Fox").transform.position;
                    if (playerPos != null)
                    {
                        transform.LookAt(new Vector3(playerPos.x, transform.position.y, playerPos.z));
                    }
                }
                if (rand < .3)
                {
                    DisplayBehavior(birdBehaviors.sing);
                }
                else if (rand < .5)
                {
                    DisplayBehavior(birdBehaviors.peck);
                }
                else if (rand < .6)
                {
                    DisplayBehavior(birdBehaviors.preen);
                }
                else if (rand < .7)
                {
                    DisplayBehavior(birdBehaviors.ruffle);
                }
                else
                {
                    DisplayBehavior(birdBehaviors.sing);
                }
                //lets alter the agitation level of the brid so it uses a different mix of idle animation next time
                anim.SetFloat("IdleAgitated", Random.value);
            }
        }
    }
    void DisplayBehavior(birdBehaviors behavior)
    {
        idle = false;
        switch (behavior)
        {
            case birdBehaviors.sing:
                anim.SetTrigger(singTriggerHash);
                break;
            case birdBehaviors.ruffle:
                anim.SetTrigger(ruffleBoolHash);
                break;
            case birdBehaviors.preen:
                anim.SetTrigger(preenBoolHash);
                break;
            case birdBehaviors.peck:
                anim.SetTrigger(peckBoolHash);
                break;
        }
    }
    void PlaySong()
    {
        if (!dead)
        {
            if (Random.value < .5)
            {
                GetComponent<AudioSource>().PlayOneShot(song1, 1);
            }
            else
            {
                GetComponent<AudioSource>().PlayOneShot(song2, 1);
            }
        }
    }

    void Update()
    {
        if (onGround && !paused && !dead)
        {
            OnGroundBehaviors();
        }
        if (interactionPhase > 0)
        {

        }
    }

    public void Interact()
    {
        switch (interactionPhase)
        {
            case 0:
                if (camDestTransform != null)
                {

                }
                else
                {
                    interactionPhase = 1;
                    Interact();
                }
                break;
            case 1:
                GetComponent<AudioSource>().PlayOneShot(caw, 1);
                anim.SetTrigger(worriedAnimationHash);
                break;
        }
    }

    public bool Condition()
    { return true; }

    public Transform getTransform()
    { return transform; }

    public string AlternativeText()
    {
        return "Crow";
    }
}
