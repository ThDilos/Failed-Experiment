using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    [SerializeField] GameObject settingPage;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip buttonClickSound;

    public void playGame()
    {
        PlayClickSound();
        SceneManager.LoadScene(1);
    }

    public void goToSettings()
    {
        PlayClickSound();
        StartCoroutine(SwitchToSettings());
    }

    public void quitGame()
    {
        PlayClickSound();
        Application.Quit();
    }
    private void PlayClickSound()
    {
        if (audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }
    private IEnumerator SwitchToSettings()
    {
        yield return new WaitForSeconds(0.2f);
        this.gameObject.SetActive(false);
        settingPage.SetActive(true);
    }
}
