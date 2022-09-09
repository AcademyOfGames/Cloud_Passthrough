using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
public class BirdStateChanger : MonoBehaviour
{
    OVRInput.Controller controllerR = OVRInput.Controller.RTouch;
    OVRInput.Controller controllerL = OVRInput.Controller.LTouch;

    public TextMeshProUGUI stateText;
    public GameObject ghostHand;

    public bool customControlsUnlocked;

    public StumpBehavior stump;

    // States
    public enum BirdState
    {
        Hunting, Welcoming, Exiting, Orbiting, GoToLanding, Landed,
        Landing,
        TakeOff,
        Diving,
        Eating
    }

    // Start with Hunting
    public BirdState currentState = BirdState.GoToLanding;

    //BirdSettings class, a group of data we call BirdSettings. Just like we can do Trasform.position to access some data. we can say BirdSettings.turnAngleIntensity
    public class BirdSettings
    {
        public float turnAngleIntensity;
        public float waypointRadius;
        public float waypointProximity;
        public float speed;
        public float turnSpeed;

        //** This is called a constructor, when you write new BirdSettings(...) in your code you can create a new BirdSettings object with specific data
        public BirdSettings(float tAngle, float wRadius, float wProximity, float vSpeed, float tSpeed )
        {
            turnAngleIntensity = tAngle;
            waypointRadius = wRadius;
            waypointProximity = wProximity;
            speed =vSpeed ;
            turnSpeed = tSpeed;
        }
    }

    private BirdSettings huntingSettings; 
    private BirdSettings welcomingSettings; 
    private BirdSettings goToLandingSettings; 
    private BirdSettings LandedSettings; 
    private BirdSettings exitingSettings;
    private BirdSettings divingSettings;

    private BirdMovement bird;
    private static readonly int TakeOff = Animator.StringToHash("TakeOff");

    public BirdStateChanger(bool customControlsUnlocked)
    {
        this.customControlsUnlocked = customControlsUnlocked;
    }

    private void Awake()
    {
            bird = GetComponent<BirdMovement>();
        SetBirdSettings();
    }

    
    // Change the bird from flying to meeting player to leaving
    public void SwitchState(BirdState birdState)
    {
        if (currentState == birdState) return;
        switch (birdState)
        {
            case BirdState.Hunting:
                bird.UpdateSettings(huntingSettings);
                break;

            case BirdState.Welcoming:
                bird.currentWaypoint = bird.player.position + Vector3.up *.5f;
                //after 1 second set the bird to welcoming
                bird.UpdateSettings(welcomingSettings);
                break;

            case BirdState.GoToLanding:
                bird.UpdateSettings(goToLandingSettings);

                break;
            case BirdState.Landing:
                bird.SwitchAnimationState(birdState);
                bird.UpdateSettings(LandedSettings);

                if (bird.grabbedFish)
                {
                    SwitchState(BirdState.Eating);
                    IEnumerator facePlayer = bird.FacePlayer();
                    StartCoroutine(facePlayer);
                    if (bird.fishCaught == 3)
                    {
                        ghostHand.SetActive(true);
                    }
                }
        
                if (bird.landingSpot == bird.handLandingSpot)
                {
                    customControlsUnlocked = true;
                    foreach (var g in bird.customControlsUI)
                    {
                        g.SetActive(true);
                    }
                }
                //Invoke("TakeOff",6);  
                break;
            
            case BirdState.TakeOff:
                bird.SwitchAnimationState(birdState);
                SwitchState(BirdState.Hunting);
                break;
            
            case BirdState.Exiting:
                break;
            
            case BirdState.Diving:
                bird.SwitchAnimationState(birdState);
                bird.prey.gameObject.SetActive(true);
                bird.UpdateSettings(divingSettings);
                //StartCoroutine("ResetToHunting");
                break;
            
            case BirdState.Eating:
                bird.SwitchAnimationState(birdState);
                //StartCoroutine("ResetToHunting");
                break;
        }
        stateText.text = birdState.ToString();

        currentState = birdState;
    }

    // Update is called once per frame
    void Update()
    {
        if (customControlsUnlocked)
        {
            // take off left trigger and landing right trigger. right A add new fish. X slow mo and Y for tricks 

            if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch) || Keyboard.current[Key.S].wasPressedThisFrame)
            {
                stump.SpawnMorePrey();
            }
            if(OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch) || Keyboard.current[Key.W].wasPressedThisFrame)
            {
                SwitchState(BirdState.Welcoming);
            }
            
            if(OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.LTouch) || Keyboard.current[Key.H].wasPressedThisFrame)
            {
                FindObjectOfType<FeedbackLogic>().StartFeedback();

                if (currentState == BirdState.Landing)
                {
                    bird.anim.SetTrigger(TakeOff);
                    SwitchState(BirdState.TakeOff);
                }
                else
                {
                    SwitchState(BirdState.Hunting);
                }
            }
            if(OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch) || Keyboard.current[Key.L].wasPressedThisFrame)
            {
                bird.landingSpot = GetComponent<BirdMovement>().handLandingSpot;
                SwitchState(BirdState.GoToLanding);
            }    
            if(OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch) || Keyboard.current[Key.T].wasPressedThisFrame)
            {
                bird.anim.SetTrigger(TakeOff);
                SwitchState(BirdState.TakeOff);
            }
        }

        /*
        if(OVRInput.GetDown(OVRInput.Button.One, controllerL) || Keyboard.current[Key.C].wasPressedThisFrame)
        {       
            print("Pressing diving");
    
            SwitchState(BirdState.Diving);
        }*/
    }
    
    void SetBirdSettings()
    {
        //**creating each bird setting for us to use. we can add custom speed, waypoint logic etc
        huntingSettings = new BirdSettings(bird.turnAngleIntensity, bird.waypointRadius, bird.waypointProximity, bird.speed, bird.turnSpeed);
        divingSettings = new BirdSettings(bird.turnAngleIntensity, bird.waypointRadius*3f, 1f, bird.speed, bird.turnSpeed *1.5f);
        welcomingSettings = new BirdSettings(0f, bird.waypointRadius, 1.2f, bird.speed * 1.3f, bird.turnSpeed * 1.5f);
        goToLandingSettings = new BirdSettings(0f, bird.waypointRadius,  .6f, bird.speed * 1.3f, bird.turnSpeed );
        exitingSettings = new BirdSettings(0f, bird.waypointRadius, 0, bird.speed * 1.3f, bird.turnSpeed);
        LandedSettings = new BirdSettings(0f, bird.waypointRadius, 0, 0f, bird.turnSpeed);
    }
}
