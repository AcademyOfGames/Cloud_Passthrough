using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour
{
    public  GameObject clouds;
    public ParticleSystem ps;
    private ParticleSystem.Particle[] particles;
    public StumpBehavior stump;
    public GameObject windGust;
    public GameObject rain;
    
    public OVRPassthroughLayer _passthroughLayer;
    private AudioManager audio;

    private void Start()
    {
        audio = FindObjectOfType<AudioManager>();
        SetDarkSky();
    }

    IEnumerator DarkenSky()
    {
        print("Darkening Sky");
        float timePassed = 0;
        float newContrast = 0;
        float newBrightness = 0;
        
        while (timePassed < 1)
        {
            timePassed += Time.deltaTime * .03f;

            newContrast = Mathf.Lerp(newContrast, .3f, timePassed);
            newBrightness = Mathf.Lerp(newBrightness, -.4f, timePassed);
            _passthroughLayer.colorMapEditorContrast = newContrast;
            _passthroughLayer.colorMapEditorBrightness = newBrightness;
            yield return new WaitForFixedUpdate();
        }
    }

    public IEnumerator ResetSky()
    {
        print("Resetting Sky");

        yield return new WaitForSeconds(1);
        float timePassed = 0;
        float newContrast = -.1f;
        float newBrightness = -.04f;
        
        while (timePassed < 1)
        {
            timePassed += Time.deltaTime * .03f;

            newContrast = Mathf.Lerp( .3f,newContrast, timePassed);
            newBrightness = Mathf.Lerp(-.4f,newBrightness,  timePassed);
            _passthroughLayer.colorMapEditorContrast = newContrast;
            _passthroughLayer.colorMapEditorBrightness = newBrightness;            
            yield return new WaitForFixedUpdate();
        }
    }

    public void SetDarkSky()
    {
        float newContrast = -.04f;
        float newBrightness = -1f;
        _passthroughLayer.colorMapEditorContrast = newContrast;
        _passthroughLayer.colorMapEditorBrightness = newBrightness;
    }
    
    public void StartTornado()
    {
        windGust.SetActive(true);
        StartCoroutine(nameof(DarkenSky));
        
        StartCoroutine(nameof(ShrinkMist));
        FindObjectOfType<SoundtrackPlayer>().PlaySound("tornadoSong");
    }

    public IEnumerator ShrinkMist()
    {
   
        int particleCount = ps.particleCount;
        particles = new ParticleSystem.Particle[particleCount];
        ps.GetParticles(particles);
        
        float timePassed = 0;

        float newY = transform.localScale.y;
        Vector3 localScale;
        BirdAudioManager birdAudio = FindObjectOfType<BirdAudioManager>();
        while(timePassed < 1)
        {
            birdAudio.SetVolume("strongWind" , birdAudio.GetVolume("strongWind" ) *.96f);
            timePassed += Time.deltaTime *.1f;

            yield return new WaitForFixedUpdate();
        }
        
        ParticleSystem.MainModule tempPS = ps.main;
        
        var main = ps.main;
        main.startLifetime = 0;
        clouds.SetActive(true);
        
        print("Deactivated mist");

        yield return new WaitForSeconds(5);

        stump.ActivateBeeSystem();
        ps.Stop();
    }

    public void StartRain()
    {
        audio.PlaySound("thunderSound");
        IEnumerator waitAndStartFeedback = FindObjectOfType<FeedbackLogic>().WaitAndStartFeedback();
        StartCoroutine(waitAndStartFeedback);
        StartCoroutine(nameof(WaitAndStartRain));
        StartCoroutine(nameof(DarkenSky));
        


    }

    IEnumerator WaitAndStartRain()
    {
        yield return new WaitForSeconds(4);
        rain.SetActive(true);
        audio.PlaySound("rainSound", true);

        IEnumerator waitAndGoAway = FindObjectOfType<BeeSystem>().WaitAndGoAway();
        StartCoroutine(waitAndGoAway);

        FindObjectOfType<SoundtrackPlayer>().PlaySound("rainSong");
    }

}
