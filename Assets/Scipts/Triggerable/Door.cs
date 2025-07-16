using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool leftDoor = true; // Is it a left door? Right door will have the opposite move direction
    public Vector3 moveDir = new Vector3(-1, 0, 0); // Direction it moves, by default in negative x direction (Vector3.Left)
    public float moveDistance = 3.0f; // Distance the door gonna move
    public float duration = 3.0f; // Time of opening sequence, prob not precise

    private float currentTime = 0.0f;
    private bool triggered = false;
    private Vector3 targetPos;
    private Vector3 originPos;
    private bool opened = false;

    private Animator animator; // For animated doors
    [Header("If an Animator is present, overriding Above")]
    [SerializeField] private string statusBool;
    [SerializeField] private bool oneTime = false;
    // Start is called before the first frame update
    public void Trigger()
    {
        if (animator == null)
        {
            if (triggered) return; // To avoid spamming interaction and sending the doors flying somehow
            if (opened)
            {
                Debug.Log("Closing Door");
                targetPos = originPos; // Set Close Door target position
            }
            else
            {
                Debug.Log("Openning Door");
                targetPos = transform.position + (leftDoor ? moveDir : -moveDir) * moveDistance; // Set Open Door target position
            }
            triggered = true;
        }
        else
        {
            animator.SetBool(statusBool, !animator.GetBool(statusBool));
            if (oneTime) Destroy(this);
        }
    }

    void Start()
    {
        originPos = transform.position; // Store the original position (Before Open)
        try
        {
            animator = GetComponent<Animator>();
        }
        catch 
        { }
    }

    void Update() // This only animates the door open
    {
        if (triggered)
        {
            currentTime += Time.fixedDeltaTime;
            float t = Mathf.Clamp01(currentTime * Time.fixedDeltaTime / duration); // t will get larger as currentTime close to duration setted..? i have no idea.. but it works smoothly
            transform.position = Vector3.Lerp(transform.position, targetPos, t); // The door move part: set position
            if (Vector3.Distance(transform.position, targetPos) < 0.01f) // This is to directly shut the door when it is closed to being fully closed, otherwise it's closing forever but never fully closed
            {
                transform.position = targetPos;
                // Reset the necessary variables and register respective states
                triggered = false;
                opened = !opened;
                currentTime = 0.0f;
            }
        }    
    }
}
