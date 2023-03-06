using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FireBehaviour : MonoBehaviour
{
    [SerializeField] List<ParticleSystem> particles;
    [SerializeField] ParticleSystem fire;

    [Header("Wood")]
    [SerializeField] Renderer bonfireRenderer;
    [SerializeField] Texture offTexture;
    [SerializeField] Texture onTexture;

    [Header("Flame")]
    // Textures for particle system material.
    [SerializeField] Texture blackFireTexture;
    [SerializeField] Texture fireTexture;
    MenuSwitch menuSwitch;

    private void Awake()
    {
        menuSwitch = FindObjectOfType<MenuSwitch>(); 
    }

    private void Start()
    {
        if (bonfireRenderer == null)
            Debug.LogError("Bonfire Renderer not found");
    }

    private void OnEnable()
    {
        menuSwitch.onMenuSwitched.AddListener(() => SetFire(menuSwitch.SwitchOn));
    }

    private void OnDisable()
    {
        menuSwitch.onMenuSwitched.RemoveAllListeners();
    }

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
        Color flameColor = new Color(1f, 1f, 1f, 1f);
        Texture woodTexture = onTexture;
        //Texture newFireTexture = fireTexture;
        if (!on)
        {
            woodTexture = offTexture;
            //flameColor = Color.white;
            //newFireTexture = blackFireTexture;
            flameColor = new Color(1f, 1f, 1f, 0.5f);
            size = 0f;
        }
        ParticleSystemRenderer rend = fire.GetComponent<ParticleSystemRenderer>();
        rend.maxParticleSize = size;
        Material fireMat = fire.GetComponent<ParticleSystemRenderer>().sharedMaterial;
        fireMat.SetColor("_TintColor", flameColor);
        //fireMat.SetTexture("_MainTex", newFireTexture);
        foreach(Material mat in bonfireRenderer.materials)
        {
            mat.SetTexture("_MainTexture", woodTexture);
        }
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

