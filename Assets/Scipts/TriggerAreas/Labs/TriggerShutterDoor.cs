using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerShutterDoor : MonoBehaviour
{
    [SerializeField]
    private ShutterDoor[] triggerDoors; // Doors to be triggered

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            if (triggerDoors.Length > 0)
            {
                foreach (ShutterDoor triggerable in triggerDoors)
                {
                    triggerable.Trigger();
                    Destroy(this); // Remove this trigger so it won't get triggered again
                }
            }
        }
        Debug.Log("Shutter Doors Triggered");
    }
}
