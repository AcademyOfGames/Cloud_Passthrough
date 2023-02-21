using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class StoryParts : MonoBehaviour
{
    [HideInInspector]public bool introSequenceStarted;
    [HideInInspector]public bool introSequenceDone;
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
        FindObjectOfType<SoundtrackPlayer>().PlaySound("Good Goodbye");
        FindObjectOfType<BirdMovement>().BirdScream();

        title.SetActive(true);

        yield return new WaitForSeconds(0.5f);
        _birdStateChanger.SwitchState(BirdStateChanger.BirdState.TakeOff);

        yield return new WaitForSeconds(15f);

        _birdStateChanger.SwitchState(BirdStateChanger.BirdState.Welcoming);

        
        /*FindObjectOfType<SoundtrackPlayer>().PlaySound("Good Goodbye");

        yield return new WaitForSeconds(3f);

        title.SetActive(true);

        yield return new WaitForSeconds(4f);

        FindObjectOfType<BirdMovement>().BirdScream();

        yield return new WaitForSeconds(2f);

        _birdStateChanger.SwitchState(BirdStateChanger.BirdState.TakeOff);

        yield return new WaitForSeconds(15f);

        _birdStateChanger.SwitchState(BirdStateChanger.BirdState.Welcoming);
        */
    }
    
    
    
}
