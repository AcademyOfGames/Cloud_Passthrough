using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour
{
    public BeeSystem beesAndFlowers;
    [HideInInspector] public AudioSource windAudio;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }
    public IEnumerator ShrinkMist()
    {

        print("now shrink mist");

        float timePassed = 0;

        float newY = transform.localScale.y;
        Vector3 localScale;
        BirdAudioManager birdAudio = FindObjectOfType<BirdAudioManager>();
        while(timePassed < 1)
        {
            birdAudio.SetVolume("strongWind" , birdAudio.GetVolume("strongWind" ) *.96f);
            timePassed += Time.deltaTime *.5f;
            localScale = transform.localScale;
            print("localScale" + localScale);

            newY = Mathf.Lerp(localScale.y, 0, timePassed);
            localScale.y = newY;
            
            transform.localScale = localScale;
            yield return null;
        }

        print("Deactivated mist");
        beesAndFlowers.gameObject.SetActive(true);
        beesAndFlowers.ActivateBeeSystem();
        gameObject.SetActive(false);
    }
}
