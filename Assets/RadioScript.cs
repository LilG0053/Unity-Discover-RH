using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadioScript : MonoBehaviour
{
    private AudioSource audio;
    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
        audio.Pause();
    }

    public void radioClick()
    {
        if (audio.isPlaying)
        {
            audio.Pause();
        } else
        {
            audio.Play();
        }
    }
}
