using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdAudioManager : MonoBehaviour
{
    public Sound[] sounds;

    private void Awake()
    {
        foreach (Sound s in sounds)
        { 
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    public void PlaySound(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.clip.name == name);
        if (s.source != null)
        {
            s.source.Play();

        }
        else
        {
            print("Couldnt find sound named " + name);
        }
    }
    
    public float GetVolume(string soundName)
    {
        Sound s = Array.Find(sounds, sound => sound.clip.name == soundName);
        if (s.source != null)
        {
           return  s.source.volume;
        }
        
        print("Couldnt find sound named " + name);
        return .5f;
    } 
    public void SetVolume(string soundName, float volume)
    {
        Sound s = Array.Find(sounds, sound => sound.clip.name == soundName);
        if (s.source != null)
        {
            s.source.volume = volume;

        }
        else
        {
            print("Couldnt find sound named " + name);
        }
    }

}
