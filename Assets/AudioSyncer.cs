using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AudioSyncer : MonoBehaviour
{
    [Tooltip("Threshold for when a beat is registered.")]
    public float bias;
    
    [Tooltip("Minimum time between beats.")]
    public float timeStep;
    
    [Tooltip("How long it takes for target to reach beat size")]
    public float timeToBeat;
    
    [Tooltip("How long it takes for target to go back to rest.")]
    public float restSmoothTime;

    private float m_previousAudioValue;
    private float m_audioValue;
    private float m_timer;

    protected bool m_isBeat;
    // Start is called before the first frame update
    void Start()
    {
    }

    public virtual void OnBeat()
    {
        m_timer = 0;
        m_isBeat = true;
    }
    public virtual void OnUpdate()
    {

        m_previousAudioValue = m_audioValue;
        m_audioValue = AudioSpectrum.spectrumValue;
        if (m_previousAudioValue > bias &&
            m_audioValue <= bias)
        {
            if (m_timer > timeStep)
            {
                OnBeat();
            }
        }

        if (m_previousAudioValue <= bias &&
            m_audioValue > bias)
        {
            if (m_timer > timeStep)
            {
                OnBeat();
            }
        }

        m_timer += Time.deltaTime;
    }
    // Update is called once per frame
    void Update()
    {
        OnUpdate();
    }
}
