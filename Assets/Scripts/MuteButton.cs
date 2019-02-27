using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MuteButton : MonoBehaviour
{
    public Sprite MuteSprite;
    public Sprite SoundSprite;
    public Button MyButton;

    public void ToggleMute()
    {
        if (PlayerPrefs.GetInt("Muted") == 0)
        {
            PlayerPrefs.SetInt("Muted", 1);
            MyButton.image.sprite = MuteSprite;
        }
        else
        {
            PlayerPrefs.SetInt("Muted", 0);
            MyButton.image.sprite = SoundSprite;
        }
    }
}
