using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeachJumpQuestStep : QuestStep
{
    public KeyCode keyToCount = KeyCode.Space;  // You can change this in the Inspector
    private int pressCount = 0;
    public int maxPressCount = 10; // Maximum number of presses to count

    private void Start()
    {
        string status = "Press space to jump";
        ChangeState("", status);
    }

    void Update()
    {
        if (Input.GetKeyDown(keyToCount))
        {
            pressCount++;
            Debug.Log($"{keyToCount} was pressed {pressCount} times.");
        }

        if (pressCount == maxPressCount) 
        {
            FinishQuestStep();
        }
    }
    protected override void SetQuestStepState(string state)
    {
        //no state is needed
    }
}
