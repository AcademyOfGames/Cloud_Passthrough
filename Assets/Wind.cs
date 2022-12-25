using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour
{
    public  GameObject clouds;
    ParticleSystem ps;
    private ParticleSystem.Particle[] particles;
    public StumpBehavior stump;
    public GameObject windGust;
    public GameObject rain;
    
    public OVRPassthroughLayer _passthroughLayer;

    private void Start()
    {
        ps = GetComponent<ParticleSystem>();
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
            newBrightness = Mathf.Lerp(newBrightness, -.3f, timePassed);
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
            newBrightness = Mathf.Lerp(-.6f,newBrightness,  timePassed);
            _passthroughLayer.SetBrightnessContrastSaturation();
            yield return new WaitForFixedUpdate();
        }
    }
    
    public void StartTornado()
    {
        windGust.SetActive(true);
        StartCoroutine(nameof(DarkenSky));
        
        windGust.SetActive(true);
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
            //localScale = transform.localScale;
            //print("localScale" + localScale);

            //newY = Mathf.Lerp(localScale.y, 0, timePassed);
            //localScale.y = newY;
           
            //transform.localScale = localScale;
            
            /*
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i].startSize *= 1.002f;
                //particles[i].velocity *= .98f;

            }
 
            ps.SetParticles(particles, particleCount);
            */
            yield return new WaitForFixedUpdate();
        }
        
        ParticleSystem.MainModule tempPS = ps.main;
        var main = ps.main;
        main.startLifetime = 0;
        clouds.SetActive(true);
        
        print("Deactivated mist");
        stump.ActivateBeeSystem();
        GetComponent<ParticleSystem>().Stop();
    }

    public void StartRain()
    {
        IEnumerator waitAndStartFeedback = FindObjectOfType<FeedbackLogic>().WaitAndStartFeedback();
        StartCoroutine(waitAndStartFeedback);
        StartCoroutine(nameof(DarkenSky));
        
        rain.SetActive(true);
        FindObjectOfType<AudioManager>().PlaySound("rainSound", true);
        FindObjectOfType<SoundtrackPlayer>().PlaySound("rainSong");

    }


}
