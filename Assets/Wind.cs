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
    private void Start()
    {
        ps = GetComponent<ParticleSystem>();
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
            
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i].startSize *= 1.003f;
                //particles[i].velocity *= .98f;

            }
 
            ps.SetParticles(particles, particleCount);
            
            yield return new WaitForFixedUpdate();
        }
        
        ParticleSystem.MainModule tempPS = ps.main;
        var main = ps.main;
        main.startLifetime = 0;
        clouds.SetActive(true);
        
        print("Deactivated mist");
        stump.ActivateBeeSystem();
        gameObject.SetActive(false);
    }

    public void StartRain()
    {
        IEnumerator waitAndStartFeedback = FindObjectOfType<FeedbackLogic>().WaitAndStartFeedback();
        StartCoroutine(waitAndStartFeedback);

    }


}
