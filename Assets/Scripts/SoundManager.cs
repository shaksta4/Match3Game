using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    //vars
    public static SoundManager instance = null;
    public AudioSource SfxSource, MusicSource;

    void Awake()
    {
        //singleton
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        
        //Initially unmuted.
        PlayerPrefs.SetInt("Muted", 0);

        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        // If muted
        if(PlayerPrefs.GetInt("Muted") == 1)
        {
            //If music is playing, pause it.
            if (MusicSource.isPlaying)
            {
                print("Muting!!");
                MusicSource.Pause();
            }
        }
        //If not muted
        else
        {
            //If it isn't playing, play it.
            if (!MusicSource.isPlaying)
            {
                print("Playing!!");
                MusicSource.Play();
            }
        }
    }

    /*This function takes in an audioclip and plays it. If muted, volume is 0, else its 50*/
    public void PlaySingle(AudioClip clip)
    {
        if(PlayerPrefs.GetInt("Muted") == 1)
        {
            SfxSource.volume = 0;
        }
        else
        {
            SfxSource.volume = 0.5f;
        }

        SfxSource.clip = clip;
        SfxSource.Play();
    }
}
