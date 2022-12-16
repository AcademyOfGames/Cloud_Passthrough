using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour
{
    public IEnumerator ShrinkMist()
    {
        print("5 seconds  shrink mist");

        yield return new WaitForSeconds(5f);
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
        FindObjectOfType<StumpBehavior>().ActivateBeeSystem();
        gameObject.SetActive(false);
    }
}
