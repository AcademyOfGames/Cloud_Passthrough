using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MicInput : MonoBehaviour
{
    private AudioClip clip;
    private AudioSource audio;

    public AudioMixerGroup micGroup;
    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
        audio.outputAudioMixerGroup = micGroup;
        audio.clip = Microphone.Start(Microphone.devices[0], true, 10, 44100);
        audio.loop = true;
        
        audio.Play();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Microphone.GetPosition(Microphone.devices[0]) > 0)
        {
            
        }
    }

}
