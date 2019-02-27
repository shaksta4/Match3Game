using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource SfxSource;
    public AudioSource MusicSource;
    public static SoundManager instance = null;
    public GameObject mutebutton;
    public static bool isMuted;

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
        
        PlayerPrefs.SetInt("Muted", 0);

        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if(PlayerPrefs.GetInt("Muted") == 1)
        {
            if (MusicSource.isPlaying)
            {
                print("Muting!!");
                MusicSource.Pause();
            }
        }
        else
        {
            if (!MusicSource.isPlaying)
            {
                print("Playing!!");
                MusicSource.Play();
            }
        }
    }

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
