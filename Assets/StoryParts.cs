using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class StoryParts : MonoBehaviour
{
    public bool introSequenceStarted;
    public bool introSequenceDone;
    private BirdStateChanger _birdStateChanger;
    public GameObject title;    
    [HideInInspector]public bool firstWelcomeDone;
    
    // Start is called before the first frame update
    void Start()
    {
    }

    public void StartIntroSequence()
    {
        _birdStateChanger = FindObjectOfType<BirdStateChanger>();

        if (!introSequenceStarted)
        {
            print("Intro sequence started");
            introSequenceStarted = true;
            StartCoroutine(nameof(IntroSequence));
        }
    }
        
    IEnumerator IntroSequence()
    {
        title.SetActive(true);
        FindObjectOfType<SoundtrackPlayer>().PlaySound("introSong");

        yield return new WaitForSeconds(6f);

        FindObjectOfType<BirdMovement>().BirdScream();

        yield return new WaitForSeconds(2f);

        _birdStateChanger.SwitchState(BirdStateChanger.BirdState.TakeOff);

        yield return new WaitForSeconds(15f);

        _birdStateChanger.SwitchState(BirdStateChanger.BirdState.Welcoming);
        

    }
}
