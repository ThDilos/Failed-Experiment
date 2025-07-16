using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShutterDoor : MonoBehaviour
{
    [SerializeField]
    private float distance = 8.0f; // Distance that the door move down
    [SerializeField]
    private float duration = 5.0f; // Rough time taken for door to shut
    [SerializeField]
    private float startDelay = 0; // Set this to negative for a delayed start

    [SerializeField] private AudioClip shutterDoorSound;
    private bool hasPlayedSound = false;

    private bool triggered = false;
    private Vector3 targetPos = Vector3.zero;

    public void Trigger()
    {
        triggered = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        startDelay *= -1;
        targetPos = transform.position + Vector3.down * distance;
    }

    // Update is called once per frame
    void Update()
    {
        if (triggered)
        {
            startDelay += Time.fixedDeltaTime;
            if (startDelay > 0.0f)
            {
                if (!hasPlayedSound)
                {
                    // play sound FX once when door starts moving
                    if (shutterDoorSound != null)
                    {
                        SoundFXManager.instance.PlaySoundFXClip(shutterDoorSound, transform, 1f);
                    }
                    hasPlayedSound = true;
                }

                float t = Mathf.Clamp01(startDelay * Time.fixedDeltaTime / duration); // t will get larger as currentTime close to duration setted..? i have no idea.. but it works smoothly
                transform.position = Vector3.Lerp(transform.position, targetPos, t); // The door move part: set position
                
                if (Vector3.Distance(transform.position, targetPos) < 0.01f) // This is to directly shut the door when it is closed to being fully closed, otherwise it's closing forever but never fully closed
                {
                    transform.position = targetPos;
                }
            }
        }
    }
}
