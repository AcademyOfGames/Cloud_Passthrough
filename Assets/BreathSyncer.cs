using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class BreathSyncer : MonoBehaviour
{
    [Tooltip("Threshold for when a beat is registered.")]
    public float bias;
    
    [Tooltip("Minimum time between beats.")]
    public float timeStep;

    [Tooltip("Interpolation of moevement.")]
    public float smoothTime;

    private float m_previousAudioValue;
    public float m_audioValue;
    private float m_timer;

    protected bool m_isBeat;
    public float grassMovementStrength;
    public enum Breath {In,Out,Neutral}
    public Breath _breathStatus = Breath.Neutral;
    private Breath _previousBreath = Breath.In;

    public float breathScaleSpeed;
    public float _breathDirection = 1f;

    private Vector3 targetScale;

    public Vector3 topScale;
    public Vector3 lowerScale;

    public TerrainData terrain; 
    private AudioSpectrum spectrum;

    //public Text debug;
    // Start is called before the first frame update
    void Start()
    {
        spectrum = GetComponent<AudioSpectrum>();
        targetScale = transform.localScale;
        terrain.wavingGrassSpeed = 1;
    }

    public virtual void OnBeat()
    {
        m_timer = 0;
        m_isBeat = true;
    }
    public virtual void FinishBreath()
    {
        targetScale = transform.localScale;
        m_timer = 0;
        _breathDirection = 0;
        _breathStatus = Breath.Neutral;
    } 
    public virtual void OnUpdate()
    {
        m_previousAudioValue = m_audioValue;
        m_audioValue = spectrum.avgSpectrumValue;
       // debug.text = "m_audioValue " + m_audioValue + " " + terrain.wavingGrassStrength;

        if (m_previousAudioValue > bias &&
            m_audioValue <= bias)
        {
            if (m_timer > timeStep && _breathStatus != Breath.Neutral)
            {
                FinishBreath();
            }
        }

        if (m_previousAudioValue <= bias &&
            m_audioValue > bias)
        {
            if (m_timer > timeStep && _breathStatus == Breath.Neutral)
            {
                SwitchBreath();
            }
        }

        m_timer += Time.deltaTime;

        transform.localScale = Vector3.Lerp(transform.localScale, targetScale,m_timer*.1f);
        terrain.wavingGrassStrength = m_audioValue * grassMovementStrength;
        //transform.localScale += Vector3.one * m_audioValue * _breathDirection * breathScaleSpeed;
    }

    private void SwitchBreath()
    {
        if (_previousBreath == Breath.In)
        {
            _breathStatus = Breath.In;
            _previousBreath = Breath.Out;
            _breathDirection = 1;
            targetScale = topScale;
        }
        else
        {
            _breathStatus = Breath.Out;
            _previousBreath = Breath.In;
            _breathDirection = -1;
            
            targetScale = lowerScale;
        }
       // print("New breath - " + _breathStatus + "... m_timer " + m_timer + " - audio " + m_previousAudioValue );
        m_timer = 0;

    }

    // Update is called once per frame
    void Update()
    {
        OnUpdate();
    }
}
