using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerChase : MonoBehaviour
{
    [SerializeField]
    private Door[] triggerDoors; // Doors to be triggered
    [SerializeField]
    private EnermyController[] enermyControllers; // Enermy to be aggroed

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            if (triggerDoors.Length > 0)
            {
                foreach (Door triggerable in triggerDoors)
                {
                    triggerable.Trigger();
                    Destroy(this); // Remove this trigger so it won't get triggered again
                }
            }
            if (enermyControllers.Length > 0)
            {
                foreach (EnermyController enermyController in enermyControllers)
                {
                    enermyController.SetState(EnermyController.State.LOCKEDON);
                    enermyController.playerPos = collider.transform.position;
                }
            }
        }
        Debug.Log("Chase Triggered");
    }
}