using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DoorWithAnimation : MonoBehaviour, IInteractable
{
    [SerializeField] private string statusBool; // The triggerable bool from animator
    [SerializeField] private string statusTrigger; // Triggerable Trigger from animator
    [SerializeField] private bool oneTime = false; // Only run once?

    [SerializeField] private AudioClip doorOpenSound;

    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Interact()
    {
        if (statusBool != null)
        {
            animator.SetBool(statusBool, !animator.GetBool(statusBool));
        }
        else
        {
            animator.SetTrigger(statusTrigger);
        }

        // play sound FX
        if (doorOpenSound != null)
        {
            SoundFXManager.instance.PlaySoundFXClip(doorOpenSound, transform, 1f);
        }

        if (oneTime) { Destroy(this); }
    }

    public bool Condition()
    {
        return true;
    }

    public Transform getTransform()
    {
        return transform;
    }
    public string AlternativeText()
    {
        return null;
    }
}
