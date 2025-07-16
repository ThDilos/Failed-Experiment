using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerEnding : MonoBehaviour
{
    void OnTriggerEnter(Collider player)
    {
        if (player.gameObject.CompareTag("Player"))
        {
            Debug.Log("ze blu tooth device is connected err seksesfaulie");
            Destroy(this); // Remove this trigger so it won't get triggered again
            SceneManager.LoadScene(3);
        }
    }
}
