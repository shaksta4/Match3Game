using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void LoadScene(int index)
    {
        if(index == 1)
        {
            GameController.instance.Playing = true;
            GameController.instance.Menu = false;
        }
        if(index == 0)
        {
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
