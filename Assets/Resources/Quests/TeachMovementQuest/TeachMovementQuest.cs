using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TeachMovementStep : QuestStep
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

    [Header("Arrow/Target Config")]
    [SerializeField] private GameObject indicatorObject;
    [SerializeField] private string locationNumberString = "1"; // e.g., "1st", "2nd", etc.

    private void Start()
    {
        string status = "Go to arrow location";
        ChangeState("", status);

        // Show the indicator if assigned
        if (indicatorObject != null)
        {
            indicatorObject.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (indicatorObject != null)
            {
                FinishQuestStep();
                indicatorObject.SetActive(false);
            }
        }
    }


    protected override void SetQuestStepState(string state)
    {
        //no state is needed
    }
}