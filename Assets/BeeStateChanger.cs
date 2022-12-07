using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class BeeStateChanger : MonoBehaviour
{
    OVRInput.Controller controllerR = OVRInput.Controller.RTouch;
    OVRInput.Controller controllerL = OVRInput.Controller.LTouch;

    public bool customControlsUnlocked;
    public GameObject[] controlUI;

    public Vector2 lMovement;
    public Vector2 rMovement;

    private HeroBeeBehavior bee;
    private static readonly int TakeOff = Animator.StringToHash("TakeOff");
    public TextMeshPro handText;
    private void Awake()
    {
        bee = GetComponent<HeroBeeBehavior>();
    }

    public void UnlockControls(bool turnOn)
    {
        customControlsUnlocked = turnOn;
        foreach(var control in controlUI)
        {
            control.SetActive(turnOn);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (customControlsUnlocked)
        {
            /* 
             * Left Index - Fly By
             * Left Grab - Explore
             * Right A - Add Fish
             * Right Index - Take Off/Land
             * Right Grab - Grab fish
             * (Later) left Y - Barrel Roll
             * (Later) Left X - Slow Mo
             * */
            lMovement = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch) ;
            rMovement = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch) ;

            //AddFish
            if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch) || Keyboard.current[Key.S].wasPressedThisFrame)
            {
                if(bee.currentState != HeroBeeBehavior.BeeState.HandControls)
                {
                    bee.SwitchStates(HeroBeeBehavior.BeeState.HandControls);
                    handText.text = "Land";
                }
                else
                {
                    bee.SwitchStates(HeroBeeBehavior.BeeState.GoToHand);
                    handText.text = "Take Off";

                }
            }


            if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.LTouch) || Keyboard.current[Key.S].wasPressedThisFrame)
            {

            }

            if (OVRInput.GetUp(OVRInput.Button.One, OVRInput.Controller.LTouch) || Keyboard.current[Key.S].wasPressedThisFrame)
            {

            }

            if(OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch) || Keyboard.current[Key.W].wasPressedThisFrame)
            {
                print("primary index triggered");
            }
            
            if(OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.LTouch) || Keyboard.current[Key.H].wasPressedThisFrame)
            {
                
            }

            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch) ||
                Keyboard.current[Key.L].wasPressedThisFrame)
            {

                
            }

            if(OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch) || Keyboard.current[Key.T].wasPressedThisFrame)
            {

            }
        }

        /*
        if(OVRInput.GetDown(OVRInput.Button.One, controllerL) || Keyboard.current[Key.C].wasPressedThisFrame)
        {       
            print("Pressing diving");
    
            SwitchState(BirdState.Diving);
        }*/
    }

    
}
