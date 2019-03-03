using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController instance = null;

    //Public bools
    public bool End = false;
    public bool Playing = false;
    public bool Menu = false;
    public bool Paused = false;
    public bool InvalidMove = false;

    //Gameobjects
    public GameObject FooterCanvas;
    GameObject GameCanvas;
    GameObject MuteButton, ReturnButton;
    GameObject PausePanel, GamePanel, GameEndPanel;
    GameObject PauseText, InvalidText;

    void Awake()
    {
        //Object is a singleton.
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(FooterCanvas);
    }

    void Start()
    {
        MuteButton = FooterCanvas.transform.Find("MuteButton").gameObject;
        ReturnButton = FooterCanvas.transform.Find("ReturnButton").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        //If game is playing
        if(Playing)
        {
            //Find gameobjects.
            if(GameCanvas == null)
            {
                GameCanvas = GameObject.FindGameObjectWithTag("GameCanvas");
                PausePanel = GameCanvas.transform.Find("PausePanel").gameObject;

                GamePanel = GameObject.FindGameObjectWithTag("GamePanel");
                PauseText = GamePanel.transform.Find("PauseText").gameObject;
                InvalidText = GamePanel.transform.Find("IllegalText").gameObject;

                GameEndPanel = GameCanvas.transform.Find("GameEndPanel").gameObject;
            }
            //If player made invalid move
            if (InvalidMove)
            {
                print("Invalid move, starting coroutine");
                StartCoroutine(ShowPopup(2)); // Show pop up for 2 seconds.
                InvalidMove = false;
            }
            //If return button is not active
            if (!(ReturnButton.activeSelf))
            {
                ReturnButton.SetActive(true);
            }
            //If mute button is active
            if(MuteButton.activeSelf)
            {
                MuteButton.SetActive(false);
            }
            //If the game is paused, set mute button and pause panel active.
            if(Paused)
            {
                MuteButton.SetActive(true);
                PausePanel.SetActive(true);
                PauseText.SetActive(false);
            }
            //If not paused, flip
            if (!Paused)
            {
                MuteButton.SetActive(false);
                PausePanel.SetActive(false);
                PauseText.SetActive(true);
            }
            //If player presses P, pause.
            if(Input.GetKeyDown(KeyCode.P))
            {
                print("Pausing!!");
                TogglePause();
            }
            //If game end panel is active, deactivate it
            if (GameEndPanel.activeSelf)
            {
                GameEndPanel.SetActive(false);
            } 
        }
        //If user is in menu
        if(Menu)
        {
            End = false;
            //Remove return button.
            if(ReturnButton.activeSelf)
            {
                ReturnButton.SetActive(false);
            }
        }
        //If game ended
        if(End)
        {
            //Activate the game end panel
            if (!GameEndPanel.activeSelf)
            {
                GameEndPanel.SetActive(true);
            }
            End = false;
        }
    }

    /*This function toggles the pause in game by setting the timescale between 0 and 1.*/
    void TogglePause()
    {
        //If not paused, set paused
        if(Time.timeScale == 1)
        {
            print("Time scale was 1");
            Time.timeScale = 0;
            Paused = true;
        }
        //If paused, set unpaused
        else if(Time.timeScale == 0)
        {
            print("Time scale was 0");
            Time.timeScale = 1;
            Paused = false;
        }
    }

    /*This function is a coroutine that pops up the InvalidText object on the screen for 'delay' amount of seconds.*/
    IEnumerator ShowPopup(int delay)
    {
        if (!InvalidText.activeSelf)
        {
            InvalidText.SetActive(true);
        }
        yield return new WaitForSeconds(delay);
        InvalidText.SetActive(false);
    }
}
