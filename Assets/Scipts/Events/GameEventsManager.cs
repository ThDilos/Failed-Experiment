using System;
using UnityEngine;
using UnityEngine.UIElements;

public class GameEventsManager : MonoBehaviour
{
    public static GameEventsManager instance { get; private set; }
    
    public InputEvents inputEvents;
    public QuestEvents questEvents;
    //public DialogueEvents dialogueEvents;
    /*
    public PlayerEvents playerEvents;
    public GoldEvents goldEvents;
    public MiscEvents miscEvents;
    */
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one Game Events Manager in the scene.");
        }
        instance = this;

        // initialize all events
        inputEvents = new InputEvents();
        questEvents = new QuestEvents();
        //dialogueEvents = new DialogueEvents();
        /*
        playerEvents = new PlayerEvents();
        goldEvents = new GoldEvents();
        miscEvents = new MiscEvents();
        */
    }
}