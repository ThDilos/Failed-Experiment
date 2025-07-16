//Tigger text are those that goes to a certain area then get triggered out a text

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerText : MonoBehaviour
{
    public GameObject uiObject;
    private static GameObject previousuiObject;  // Store the previous UI object to deactivate it (for now like this)

    // Start is called before the first frame update
    void Start()
    {
        uiObject.SetActive(false);
        previousuiObject = null;
    }

    // Update is called once per frame
    void OnTriggerEnter (Collider player)
    {
        if (player.gameObject.CompareTag("Player"))
        {
            // If there is a previous object, deactivate it
            if (previousuiObject != null && previousuiObject.activeSelf)
            {
                //Debug.Log("Hiding previous UI object: " + previousuiObject.name);
                previousuiObject.SetActive(false);  // Hide previous instruction
            }

            // Set the current uiObject as the previous object for the next trigger
            previousuiObject = uiObject;

            //Debug.Log("Trigger Text is being captured"); 
            Debug.Log("Showing new UI object: " + uiObject.name);
            uiObject.SetActive(true);
            StartCoroutine(WaitForSec());
        }
    }

    IEnumerator WaitForSec()
    {
        yield return new WaitForSeconds(5);
        Destroy(uiObject);
        Destroy(gameObject);
    }
}
