using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController instance = null;

    public bool Playing = false;
    public bool Menu = false;
    public bool Paused = false;

    public GameObject FooterCanvas;
    GameObject mutebutton;
    GameObject returnbutton;
    GameObject pausepanel;
    GameObject gamepanel;
    GameObject pausetext;
    GameObject gamecanvas;

    void Awake()
    {
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
        mutebutton = FooterCanvas.transform.Find("MuteButton").gameObject;
        returnbutton = FooterCanvas.transform.Find("ReturnButton").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if(Playing)
        {
            if(gamecanvas == null)
            {
                gamecanvas = GameObject.FindGameObjectWithTag("GameCanvas");
                pausepanel = gamecanvas.transform.Find("PausePanel").gameObject;

                gamepanel = GameObject.FindGameObjectWithTag("GamePanel");
                pausetext = gamepanel.transform.Find("PauseText").gameObject;
            }
            if(!(returnbutton.activeSelf))
            {
                returnbutton.SetActive(true);
            }
            if(mutebutton.activeSelf)
            {
                mutebutton.SetActive(false);
            }
            if(Paused)
            {
                mutebutton.SetActive(true);
                pausepanel.SetActive(true);
                pausetext.SetActive(false);
            }
            if (!Paused)
            {
                mutebutton.SetActive(false);
                pausepanel.SetActive(false);
                pausetext.SetActive(true);
            }
            if(Input.GetKeyDown(KeyCode.Space))
            {
                print("Pausing!!");
                TogglePause();
            }
        }
    }

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
}
