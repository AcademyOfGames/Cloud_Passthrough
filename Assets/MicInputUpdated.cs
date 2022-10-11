using System;

using System.Collections;

using System.Collections.Generic;

using UnityEngine;

 

//[RequireComponent(typeof(AudioSource))]

 

public class MicInputUpdated : MonoBehaviour

{
    //AudioSource _audioSource;
    public static float MicLoudness;
    private string _device;
    private AudioClip _clipRecord = null;
    int _sampleWindow = 128;
    public float sizeIntensity;
    private Vector3 oldIntensity = new Vector3(3, 3, 3);
    public float inputSmooth;

    private void Awake()
    {

        _device = Microphone.devices[0];
        
        foreach (var v in Microphone.devices)
        {
            print(v);
        }
    }
    
    //mic initialization
    void InitMic()
    {
        if (_device == null) _device = Microphone.devices[0];
        _clipRecord = Microphone.Start(_device, true, 999, 44100);
        print("_device = " + _device);
    }

    void StopMicrophone()
    {
        Microphone.End(_device);
    }
    
    //get data from microphone into audioclip
    public float  LevelMax()

    {

        float levelMax = 0;

        float[] waveData = new float[_sampleWindow];

        int micPosition = Microphone.GetPosition(_device)-(_sampleWindow+1); // null means the first microphone

           

        if (micPosition < 0)

        {

            print("Mic pos is less than 0");

            return 0;

        }

        _clipRecord.GetData(waveData, micPosition);

        // Getting a peak on the last 128 samples

        for (int i = 0; i < _sampleWindow; i++)
        {
            float wavePeak = waveData[i] * waveData[i];
            //print("WavePeak " + wavePeak);
            if (levelMax < wavePeak) {
                levelMax = wavePeak;
            }
        }
        return levelMax;
    }

 

    void Update()
    {
        // levelMax equals to the highest normalized value power 2, a small number because < 1
        // pass the value to a static var so we can access it from anywhere
        MicLoudness = LevelMax ();
        if (MicLoudness < .5f)
        {
            Vector3 secondArg = Vector3.one * MicLoudness * sizeIntensity;
            if (oldIntensity.x < 3) oldIntensity.x = 3;
            if (oldIntensity.y < 3) oldIntensity.y = 3;
            if (oldIntensity.z < 3) oldIntensity.z = 3;

            oldIntensity = secondArg;
        }
    }

    bool _isInitialized;

    // start mic when scene starts
    void OnEnable()
    {
        InitMic();
        _isInitialized = true;
    }

    
    //stop mic when loading a new level or quit application
    void OnDisable()
    {
        StopMicrophone();
    }

    void OnDestroy()
    {
        StopMicrophone();
    }

    // make sure the mic gets started & stopped when application gets focused
    void OnApplicationFocus(bool focus) {
        if (focus)
        {
            if(!_isInitialized){
                InitMic();
                _isInitialized = true;
            }
        }     

        if (!focus)
        {
            StopMicrophone();
            _isInitialized = false;
        }

    }

}

