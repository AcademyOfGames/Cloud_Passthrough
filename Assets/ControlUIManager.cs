using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ControlUIManager : MonoBehaviour
{

    public GameObject leftEagleControls;
    public GameObject rightEagleControls;
    public GameObject seedGrabControls;

    public GameObject fishControlUI;

    public TextMeshPro grabFishText;
    public TextMeshPro grabSeedText;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void ToggleEagleControlUI(bool toggle)
    {
        print("Toggling eagle controllers to " + toggle);
        FindObjectOfType<BirdStateChanger>().customControlsUnlocked = toggle;

        leftEagleControls.SetActive(toggle);
        rightEagleControls.SetActive(toggle);
        fishControlUI.SetActive(toggle);
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeFishGrabText(string grabFish)
    {
        grabFishText.text = grabFish;
    }

    internal void TurnOnSeedControls(bool v)
    {
        seedGrabControls.SetActive(v);
    }

    public void ChangeSeedText(string text)
    {
        grabSeedText.text = text;
    }
}
