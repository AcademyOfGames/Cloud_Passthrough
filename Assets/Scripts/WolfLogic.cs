using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfLogic : MonoBehaviour
{
    private AudioSource audio;
    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    public void PlayAudio(string soundName)
    {
        audio.Play();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
