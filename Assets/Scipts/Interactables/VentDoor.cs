using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;

public class VentDoor : MonoBehaviour, IInteractable
{

    [SerializeField]
    public TriggerArea triggerArea; // Area that allows this to be triggered
    [SerializeField]
    private float force = 10.0f; // Force to apply when triggered, too much will send it flying through wall
    public Transform getTransform() { return transform; }
    private Rigidbody rb;
    private BoxCollider bc;
    private RigidbodyConstraints origin;
    private bool triggered = false;
    private float timeSinceInteracted = 0.0f;
    [SerializeField] private Vector3 flyDirection;

    [SerializeField] private AudioClip ventBreakSound;

    [SerializeField] private AudioClip alarmSound;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // Set isKinematic to true, unless Player can clip through the vent by holding movement keys
        bc = GetComponent<BoxCollider>();
        //bc.size = new Vector3(0.9f, 10, 0.9f); // Slightly extended collider size, this was the first attempt to address the clipping issue above. Maybe this can be deleted?
        //bc.center = new Vector3(0, 2f, 0); // Same as above, adjusted collider size and its center
        origin = rb.constraints; // Store the original constraints, cuz we're freezing them next
        rb.constraints = RigidbodyConstraints.FreezeAll; // Freeze all constraints to lock in place (Not pushable by player)
    }

    void Update()
    {
        // This whole sequence does one thing: After some time of being triggered, and it has stopped moving, freeze the rigidbody so it won't move weirdly
        if (triggered)
        {
            timeSinceInteracted += Time.deltaTime;
        }
        if (timeSinceInteracted > 1.0f && rb.velocity.magnitude == 0)
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
        Debug.DrawRay(transform.position, flyDirection * 2f);
    }

    public void Interact()
    {
        Destroy(triggerArea); // Remove the triggerArea. So it won't get triggered again.
        rb.isKinematic = false; // Set isKinematic to false, so it can be acted on by addForce below
        rb.constraints = origin; // Reset constraints for it to move
        //bc.size = new Vector3(1, 1, 1); // Reset the collider size and center
        //bc.center = new Vector3(0, 0, 0);
        rb.AddForce((flyDirection == null) ? Vector3.forward : flyDirection * force, ForceMode.Impulse); // Push it out
        triggered = true;

        // play sound FX
        if (ventBreakSound != null)
        {
            SoundFXManager.instance.PlaySoundFXClip(ventBreakSound, transform, 0.5f);
        }

        if (alarmSound != null)
        {
            SoundFXManager.instance.PlayLoopingSoundFXClip(alarmSound, transform, 0.4f);
        }
    }

    public bool Condition()
    {
        if (triggerArea == null) { return false; }
        return triggerArea.PlayerInRange();
    }

    public string AlternativeText()
    {
        return null;
    }
}
