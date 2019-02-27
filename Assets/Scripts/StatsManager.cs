using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsManager : MonoBehaviour
{
    public static int scoreValue = 0;
    public static int numTurns = 0;

    public Text score;
    public Text turns;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        score.text = "Score: " + scoreValue;
        turns.text = "Turns: " + numTurns;
    }
}
