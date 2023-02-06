using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HeartSyncer : MonoBehaviour
{
    [Tooltip("Threshold for when a beat is registered.")]
    public float bias;
    
    [Tooltip("Minimum time between beats.")]
    public float timeStep;

    [Tooltip("Interpolation of moevement.")]
    public float smoothTime;

    private float m_previousAudioValue;
    public float m_audioValue;
    public float m_timer;

    protected bool m_isBeat;
    public enum Beat {One,Two,Neutral}
    public Beat _currentBeat = Beat.Neutral;

    public float breathScaleSpeed;
    private Vector3 targetScale;

    public Vector3 topScale;
    public Vector3 lowerScale;

    private List<HeartButterflyMovement> butterflyAnims;

    private AudioSpectrum spectrum;
    // Start is called before the first frame update
    void Start()
    {
        butterflyAnims = new List<HeartButterflyMovement>();
        var butterflies = GameObject.FindGameObjectsWithTag("HeartButterfly");
        
        foreach (var b in butterflies)
        {
            butterflyAnims.Add(b.GetComponent<HeartButterflyMovement>());
        }
        spectrum = GetComponent<AudioSpectrum>();
        
        targetScale = transform.localScale;
    }

    public virtual void OnBeat()
    {
//        print("On Beat");
        m_timer = 0;
        m_isBeat = true;
        foreach (var b in butterflyAnims)
        {
            b.HeartBeatFly();

        }
    }

    public virtual void OnUpdate()
    {
        m_previousAudioValue = m_audioValue;
        m_audioValue = spectrum.avgSpectrumValue;
        
        if (m_previousAudioValue <= bias &&
            m_audioValue > bias)
        {
            if (m_timer > timeStep && _currentBeat == Beat.Neutral)
            {
                OnBeat();
            }
        }

        /*
        if (m_previousAudioValue <= bias &&
            m_audioValue > bias)
        {
            if (m_timer > timeStep && _breathStatus == Breath.Neutral)
            {
                SwitchBreath();
            }
        }
*/
        m_timer += Time.deltaTime;

        transform.localScale = Vector3.Lerp(transform.localScale, targetScale,m_timer*.1f);
        
    }

    
    /*
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
        
        m_timer = 0;

    }
*/
    // Update is called once per frame
    void Update()
    {
        OnUpdate();
    }
}
