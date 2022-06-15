using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class BirdStateChanger : MonoBehaviour
{
    OVRInput.Controller controllerR = OVRInput.Controller.RTouch;
    OVRInput.Controller controllerL = OVRInput.Controller.LTouch;

    public TextMeshProUGUI stateText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if(OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, controllerR))
        {
            FindObjectOfType<BirdMovement>().StartWelcomeState();
            stateText.text = "Welcoming";
        }
        if(OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, controllerL))
        {
            FindObjectOfType<BirdMovement>().SetToFlyingState();
            stateText.text = "Flying";
        }
        if(OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, controllerR))
        {
            FindObjectOfType<BirdMovement>().StartLandingState();
            stateText.text = "Landing";
        }    
        if(OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, controllerL))
        {
            FindObjectOfType<BirdMovement>().TakeOffState();
            stateText.text = "TakeOff";
        }
    }
}
