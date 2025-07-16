using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class VisitDoorQuestStep : QuestStep
{
    /*
    //if visit multiple doors
    [Header("Config")]
    [SerializeField] private string door = "Visit door";
    private void Start()
    {
        string status = "Visit the " + doorNumberString + " door";
        ChangeState("", status);
    }
   */

    private void Start()
    {
        string status = "Go to door behind obstacle ";
        ChangeState("", status);

        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            string status = "Door has been opened";
            ChangeState("", status);
            FinishQuestStep();
        }
    }


    protected override void SetQuestStepState(string state)
    {
        //no state is needed
    }
}
