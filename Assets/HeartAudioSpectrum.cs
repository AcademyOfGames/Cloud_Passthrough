using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartAudioSpectrum : MonoBehaviour
{
    private float[] m_audioSpectrum;
    public static float spectrumValue { get; private set; }
    public static float avgSpectrumValue { get; private set; }
    
    // Start is called before the first frame update
    void Start()
    {
        avgSpectrumValue = 0;
        m_audioSpectrum = new float[128];
    }

    // Update is called once per frame
    void Update()
    {
        avgSpectrumValue = 0;
        AudioListener.GetSpectrumData(m_audioSpectrum, 0, FFTWindow.Hamming);
        if (m_audioSpectrum != null && m_audioSpectrum.Length > 0)
        {
            foreach (var VARIABLE in m_audioSpectrum)
            {
                avgSpectrumValue += VARIABLE;
            }

            avgSpectrumValue /= 128;
            avgSpectrumValue *= 10000;
            spectrumValue = m_audioSpectrum[0] * 100;
            //print(spectrumValue);
        }
    }
}
