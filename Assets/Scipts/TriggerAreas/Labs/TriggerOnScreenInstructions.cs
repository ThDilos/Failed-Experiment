//currently not in use as of version 1.2
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TriggerOnScreenInstructions : MonoBehaviour
{
    [SerializeField] private GameObject instruction;
    [SerializeField] private string content;
    [SerializeField] private float alivetimer;

    private InstructionText instructionText;
    // Start is called before the first frame update
    void Start()
    {
        try
        {
            instructionText = instruction.GetComponent<InstructionText>();
        }
        catch
        {
            Debug.Log("InstructionText Component not found in " + instruction.gameObject.name + "! Plz check!");
        }
    }
    void OnTriggerEnter(Collider player)
    {
        if (player.gameObject.tag == "Player")
        {            
            instructionText.setText(content, alivetimer);
            StartCoroutine("WaitForSec1");
        }
    }


    IEnumerator WaitForSec(float timeBeforeDeactivation)
    {
        yield return new WaitForSeconds(timeBeforeDeactivation);

        Destroy(gameObject);
    }

}
