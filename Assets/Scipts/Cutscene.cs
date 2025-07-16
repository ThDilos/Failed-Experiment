using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// This script is for the image cutscene, to be deleted
public class Cutscene : MonoBehaviour
{
    [SerializeField] private Sprite[] cutscenes;
    [SerializeField] private float timeShowing = 5.0f;
    [SerializeField] private float fadingTime = 1.0f;
    [SerializeField] private int loadScene;

    private Image image;
    private int index = 1;
    private float currentTime = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        if (cutscenes.Length < 0)
        {
            Debug.Log("No Cutscenes added!!!");
        }
        else
        {
            image = GetComponent<Image>();
            image.sprite = cutscenes[0];
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (currentTime < timeShowing)
        {
            currentTime += Time.deltaTime;
        }
        else
        {
            currentTime = 0.0f;
            if (index < cutscenes.Length)
            {
                image.sprite = cutscenes[index];
                ++index;
            }
            else
            {
                SceneManager.LoadScene(loadScene);
            }
        }
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            currentTime = timeShowing;
        }
    }
}
