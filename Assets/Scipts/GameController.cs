using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameController : MonoBehaviour
{

    [SerializeField]
    private int frameRate = 60;

    [SerializeField]
    private Vector3 tpPos = Vector3.zero;
    // Start is called before the first frame update
    private void Start()
    {
        // Limit Framerate
        try { frameRate = PlayerPrefs.GetInt("FPS"); } catch { };
        QualitySettings.vSyncCount = 0; // Set vSyncCount to 0 so that using .targetFrameRate is enabled.
        Application.targetFrameRate = frameRate; // Default fps is set to 60, so that your GPU won't scream eve
    }

    public void respawn()
    {
        SceneManager.LoadScene("Starting Lab");
    }
}
