using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class TutorialHand : MonoBehaviour
{
    private static readonly int Eating = Animator.StringToHash("Eating");

    public Transform glove;
    public Transform rightHand;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<OVRGrabber>() != null)
        {
            glove.SetParent(rightHand);
            glove.localPosition = Vector3.zero;
                BirdMovement bird = FindObjectOfType<BirdMovement>();
            bird.landingSpot = bird.handLandingSpot;
            bird.anim.SetBool(Eating, false);
            bird.anim.SetTrigger("TakeOff");
            FindObjectOfType<BirdStateChanger>().SwitchState(BirdStateChanger.BirdState.GoToLanding);
            gameObject.SetActive(false);
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
