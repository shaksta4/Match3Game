using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void LoadScene(int index)
    {
        //If playing
        if(index == 1)
        {
            GameController.instance.Playing = true;
            GameController.instance.Menu = false;
            GameController.instance.End = false;
        }
        //if in Menu
        if(index == 0)
        {
            GameController.instance.End = false;
            GameController.instance.Menu = true;
            GameController.instance.Playing = false;
        }

        SceneManager.LoadScene(index);
    }


    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
