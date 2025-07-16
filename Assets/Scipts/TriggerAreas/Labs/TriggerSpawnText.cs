//Spawn text are those that comes out when spawned, literally
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSpawnText : MonoBehaviour
{
    public GameObject uiObject1;
    public GameObject uiObject2;
    private bool textAppear = false;
    // Start is called before the first frame update
    void Start()
    {
        uiObject1.SetActive(false);
        uiObject2.SetActive(false);
    }

    // Update is called once per frame
    void OnTriggerEnter(Collider player)
    {
        if (player.gameObject.tag == "Player")
        {
            //Debug.Log("Spawn text is being captured");
            uiObject1.SetActive(true);
            StartCoroutine("WaitForSec1");
        }
    }

    IEnumerator WaitForSec1()
    {
        yield return new WaitForSeconds(5);
        Destroy(uiObject1);
        StartCoroutine("WaitForSec2");
    }

    IEnumerator WaitForSec2()
    {
        yield return new WaitForSeconds(1);
        uiObject2.SetActive(true);
        yield return new WaitForSeconds(5);
        Destroy(uiObject2);
        Destroy(gameObject);
    }
}
