using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FireBehaviour : MonoBehaviour
{
    [SerializeField] List<ParticleSystem> particles;
    [SerializeField] ParticleSystem fire;

    [Header("Textures")]
    // Textures for particle system material.
    [SerializeField] Texture blackFireTexture;
    [SerializeField] Texture fireTexture;

    private void SetFire(bool on)
    {
        // particles
        foreach (ParticleSystem particle in particles)
        {
            if (!on)
            {
                // off
                if (particle.isPlaying)
                    particle.Stop(false, ParticleSystemStopBehavior.StopEmitting);
            }
            else
            {
                // normal
                if (!particle.isPlaying)
                    particle.Play();
            }
        }

        // fire
        float size = 2.7f;
        Color flameColor = new Color(1f, 1f, 1f, 0.75f);
        Texture newFireTexture = fireTexture;
        if (!on)
        {
            flameColor = Color.white;
            newFireTexture = blackFireTexture;
            size = 0.75f;
        }
        ParticleSystemRenderer rend = fire.GetComponent<ParticleSystemRenderer>();
        rend.maxParticleSize = size;
        Material fireMat = fire.GetComponent<ParticleSystemRenderer>().sharedMaterial;
        fireMat.SetColor("_TintColor", flameColor);
        fireMat.SetTexture("_MainTex", newFireTexture);
    }

    /// <summary>
    /// Turn fire on or off.
    /// </summary>
    /// <param name="onOff"></param>
    public void TurnFireOnOff(bool onOff)
    {
        SetFire(onOff);
    }

}

