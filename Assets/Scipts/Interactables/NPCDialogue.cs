using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NPCDialogue : MonoBehaviour, IInteractable
{
    GameObject dialogBox;
    Dialogue dialog;
    Transform playerTransform;
    Player playerScript;

    public List<string> lines; // The Line to display
    [Header("Progressive Lines - Repeat the following lines when 1st lines is already triggered")]
    [Tooltip("Whether the 1st sequence of lines only show once, and the next sequence of lines are repeated afterwards.")] public bool progressiveLine;
    public List<string> repeatedLines; // The Lines that are repeated
    private bool interacting = false;
    private bool alreadyInteracted = false;
    public string NPCName = null;
    private void Start()
    {
        playerTransform = GameObject.Find("Fox").transform;
        playerScript = GameObject.Find("Fox").GetComponent<Player>();
        // Use the Base Canvas as reference to find inactive Object
        GameObject canvas = GameObject.Find("DialogCanvas");
        dialogBox = canvas.transform.Find("DialogueBox").gameObject;
        dialog = dialogBox.GetComponent<Dialogue>();

        OverflowDetection();
    }

    private void Update()
    {
        if (interacting)
        {
            interacting = dialogBox.activeSelf;
            if (Vector3.Distance(playerTransform.position, transform.position) > 10.0)
                ResetDialog();
        }
    }

    public void Interact()
    {
        dialogBox.SetActive(true);
        interacting = true;
        if (!alreadyInteracted || repeatedLines.Count == 0) dialog.SetLines(lines);
        else dialog.SetLines(repeatedLines);
        alreadyInteracted = progressiveLine;
    }

    private void ResetDialog()
    {
        dialogBox.SetActive(false);

    }

    public Transform getTransform() { return transform; }

    public bool Condition()
    {
        return !dialogBox.activeSelf; 
    }

    private void OverflowDetection()
    {
        if (lines.Count == 0) return;

        for (int i = 0; i < lines.Count; i++)
        {
            string line = lines[i];
            if (line.Length > 135)
            {
                string cutted = line.Substring(135); // Gets the remaining part
                line = line.Substring(0, 135) + "-";

                lines[i] = line; // Update the current line
                lines.Insert(i + 1, cutted);
            }
        }
    }

    public string AlternativeText()
    {
        return "Chat with " + NPCName;
    }
}
