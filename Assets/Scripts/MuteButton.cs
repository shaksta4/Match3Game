using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MuteButton : MonoBehaviour
{
    //variables
    public Sprite MuteSprite, SoundSprite;
    public Button MyButton;

    /*This function toggles the mute in game by setting it in the playerprefs.*/
    public void ToggleMute()
    {
        //If not muted, mute
        if (PlayerPrefs.GetInt("Muted") == 0)
        {
            PlayerPrefs.SetInt("Muted", 1);
            MyButton.image.sprite = MuteSprite;
        }
        //else, unmute.
        else
        {
            PlayerPrefs.SetInt("Muted", 0);
            MyButton.image.sprite = SoundSprite;
        }
    }
}
