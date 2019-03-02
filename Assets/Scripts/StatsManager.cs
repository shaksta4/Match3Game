using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsManager : MonoBehaviour
{
    public int scoreValue = 0;
    public int numTurns = 0;
    public float timerValue = 0f;
    public float gameTimer = 0f;

    public Text score;
    public Text turns;
    public Text timer;
    public Text GameEndText;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.instance.Playing)
        {
            TimeSpan time = TimeSpan.FromSeconds(timerValue);

            score.text = "Score: " + scoreValue;
            turns.text = "Turns: " + numTurns;
            timer.text = "Time Left: " + time.ToString("mm':'ss");
        }
        
        if(GameController.instance.End)
        {
            TimeSpan time = TimeSpan.FromSeconds(gameTimer);
            //DO something here
            GameEndText.text = "Score: " + scoreValue + "\nTurns: "+numTurns+"\nTime played: "+time.ToString("mm':'ss");
        }
    }
}
