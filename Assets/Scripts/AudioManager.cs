using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    private AudioSource latestSound;
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

        if (s != null)
        {
            s.source.Play();
        }
        else
        {
            print("Couldnt find sound named " + name);
        }
    }

    
    public void PlaySound(string soundName, bool fadein)
    {
        Sound s = Array.Find(sounds, sound => sound.clip.name == soundName);

        if (s != null)
        {
            s.source.Play();
        }
        else
        {
            print("Couldnt find sound named " + soundName);  
            return;
        }
        
        s.source.Play();
        latestSound = s.source;
        if(fadein) StartCoroutine("FadeInSound", s.source);
    }

    IEnumerator FadeInSound(AudioSource source)
    {
        float timePassed = 0;   

        while (timePassed <= 1)
        {
            timePassed += Time.deltaTime * .3f;
            source.volume = Mathf.Lerp(0, .8f, timePassed);
            yield return new WaitForFixedUpdate();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
