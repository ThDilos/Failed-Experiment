using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingPage : MonoBehaviour
{
    [SerializeField] GameObject mainMenu;
    [SerializeField] TMP_Dropdown fps;

    public void Save()
    {
        int value = int.Parse(fps.captionText.text); 
        PlayerPrefs.SetInt("FPS", value);
        Debug.Log("FPS set to " + PlayerPrefs.GetInt("FPS"));
        Cancel();
    }
    public void Cancel()
    {
        this.gameObject.SetActive(false);
        mainMenu.SetActive(true);
    }
}
