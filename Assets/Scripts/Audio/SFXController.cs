using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXController : MonoBehaviour
{
    public AudioClip[] sfx;
    AudioSource source;

    int lastPlayed;
    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play(int i)
    {
        if (lastPlayed != i || !source.isPlaying)
        {
            source.clip = sfx[i];
            source.Play();
            lastPlayed = i;
        }
    }
}
