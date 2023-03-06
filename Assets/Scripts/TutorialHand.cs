using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class TutorialHand : MonoBehaviour
{
    private static readonly int Eating = Animator.StringToHash("Eating");

    public Transform glove;
    public Transform rightHand;
    public GameObject handParent;
    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<OVRGrabber>() != null)
        {
            EquipGlove();
        }
    }

    public void EquipGlove()
    {
        handParent.SetActive(true);
        glove.SetParent(rightHand);
        glove.localPosition = Vector3.zero;
        glove.localEulerAngles = Vector3.zero;

        BirdMovement bird = FindObjectOfType<BirdMovement>();
        if (bird != null)
        {
            bird.landingSpot = bird.handLandingSpot;
            bird.anim.SetBool(Eating, false);
            bird.anim.SetTrigger("TakeOff");
            bird.anim.ResetTrigger("Glide");
            bird.anim.ResetTrigger("Flap");
            FindObjectOfType<BirdStateChanger>().SwitchState(BirdStateChanger.BirdState.GoToLanding);
        }

        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
