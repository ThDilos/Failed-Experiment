using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InstructionText : MonoBehaviour
{
    private float aliveTimer;
    private TMP_Text text;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TMP_Text>();
        aliveTimer = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (aliveTimer > 0.0f)
        {
            aliveTimer -= Time.deltaTime;
        }
        else
        {
            text.SetText("");
        }
    }

    public void setText(string text, float aliveTimer)
    {
        this.aliveTimer = aliveTimer;
        this.text.SetText(text);
    }
}
