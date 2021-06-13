using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MusicController : MonoBehaviour
{
    public AudioSource[] songs;
    public int targetSong;

    float lastChangeTime;
    float lastUpdate;
    float timeToChange;

    // Start is called before the first frame update
    void Start()
    {
        targetSong = 0;
        lastChangeTime = 0;
        timeToChange = 8;
        songs[0].volume = 1;
        for (int i = 1; i < songs.Length; i++) {
            songs[i].volume = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        float deltaTime = Time.time - lastUpdate;
        if (Time.time - lastChangeTime < timeToChange)
        {
            float remainingTime = lastChangeTime + timeToChange - lastUpdate;
            for (int i = 0; i < songs.Length; i++)
            {
                if (targetSong == i)
                {
                    songs[i].volume += (1 - songs[i].volume) * (deltaTime / remainingTime);
                }
                else
                {
                    songs[i].volume -= songs[i].volume * (deltaTime / remainingTime);
                }
            }
        }
        else
        {
            for (int i = 0; i < songs.Length; i++)
            {
                if (targetSong == i)
                {
                    songs[i].volume = 1;
                } else
                {
                    songs[i].volume = 0;
                }
            }
        }
        lastUpdate = Time.time;
    }

    public void ChangeSong(int target)
    {
        targetSong = target;
        targetSong %= songs.Length;
        lastChangeTime = Time.time;
        if (!songs[targetSong].playOnAwake)
        {
            songs[targetSong].Play();
        }
    }

    public void ChangeSong(int target, float time)
    {
        targetSong = target;
        targetSong %= songs.Length;
        lastChangeTime = Time.time;
        if (!songs[targetSong].playOnAwake)
        {
            songs[targetSong].Play();
        }
        timeToChange = time;
        if (time == 0)
        {
            for (int i = 0; i < songs.Length; i++)
            {
                if (targetSong == i)
                {
                    songs[i].volume = 1;
                }
                else
                {
                    songs[i].volume = 0;
                }
            }
        }
    }
}
