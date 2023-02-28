using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FireBehaviour : MonoBehaviour
{
    [SerializeField] List<ParticleSystem> particles;
    [SerializeField] Renderer fireSheet;
    [SerializeField] ParticleSystem fire;

    private void SetFire(bool on)
    {
        float size = 0f;
        Color flameColor = Color.white;
        foreach (ParticleSystem particle in particles)
        {
            if (!on)
            {
                // off ish
                if (particle.isPlaying)
                    particle.Stop(false, ParticleSystemStopBehavior.StopEmitting);
                size = 0.5f;
                flameColor = new Color(0.08261337f, 0.08250264f, 0.1698113f, 0.75f);
            }
            else
            {
                // normal
                if (!particle.isPlaying)
                    particle.Play();
                size = 2.7f;
                flameColor = new Color(1f, 1f, 1f, 0.75f);
            }
        }

        ParticleSystemRenderer rend = fire.GetComponent<ParticleSystemRenderer>();
        rend.maxParticleSize = size;
        Material fireMat = fire.GetComponent<ParticleSystemRenderer>().sharedMaterial;
        fireMat.SetColor("_TintColor", flameColor);
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

