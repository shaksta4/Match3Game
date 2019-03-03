using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsManager : MonoBehaviour
{
    //variables
    public int scoreValue = 0;
    public int numTurns = 0;
    public float timerValue = 0f;
    public float gameTimer = 0f;

    public string ReasonOfLoss = "";

    public Text score, turns, timer, GameEndText;


    void Update()
    {
        //If game is playing, update the score, turns made and time left for player.
        if (GameController.instance.Playing)
        {
            TimeSpan time = TimeSpan.FromSeconds(timerValue);

            score.text = "Score: " + scoreValue;
            turns.text = "Turns: " + numTurns;
            timer.text = "Time Left: " + time.ToString("mm':'ss");
        }
        //If game ended
        else if(GameController.instance.End)
        {
            //Set the game end message.
            TimeSpan time = TimeSpan.FromSeconds(gameTimer);
            GameEndText.text = "The game ended because "+ReasonOfLoss+"\n\nScore: " + scoreValue + "\nTurns: "+numTurns+"\nTime played: "+time.ToString("mm':'ss");
        }
    }
}
