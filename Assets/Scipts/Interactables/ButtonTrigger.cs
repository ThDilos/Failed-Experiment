using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ButtonTrigger : MonoBehaviour, IInteractable
{
    [SerializeField]
    public Door[] triggerObject; // Door that it's gonna open
    [SerializeField]
    public TriggerArea triggerArea; // Area that can trigger the button

    public Transform getTransform()
    {
        return transform;
    }

    public void Interact()
    {
            foreach (Door o in triggerObject)
            {
                o.Trigger();
            }
    }

    public bool Condition()
    {
        if (triggerArea == null) { return true; }
        return triggerArea.PlayerInRange();
    }

    public string AlternativeText()
    {
        return null;
    }
}
