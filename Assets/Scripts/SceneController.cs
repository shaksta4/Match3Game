using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    /*This function takes in an integer and loads a different unity scene based on the parameter. It sets the current gamestate in game controller*/
    public void LoadScene(int index)
    {
        //If playing
        if(index == 1)
        {
            GameController.instance.Menu = false;
            GameController.instance.End = false;
            GameController.instance.Playing = true;
        }
        //if in Menu
        if(index == 0)
        {
            GameController.instance.End = false;
            GameController.instance.Playing = false;
            GameController.instance.Menu = true;
        }

        SceneManager.LoadScene(index);
    }

    /*This function holds functionality about how to quit the game based on whether its release app, or in editor*/
    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
