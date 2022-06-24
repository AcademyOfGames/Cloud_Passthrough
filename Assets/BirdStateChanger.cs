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
    
    // States
    public enum BirdState
    {
        Hunting, Welcoming, Exiting, Orbiting, GoToLanding, Landed,
        Landing,
        TakeOff,
        Catching
    }

    // Start with Hunting
    public BirdState currentState = BirdState.Hunting;

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
    private BirdSettings catchingSettings;

    private BirdMovement bird;
    // Start is called before the first frame update
    void Start()
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
                bird.SetNewSettings(huntingSettings);

                break;

            case BirdState.Welcoming:
                bird.currentWaypoint = bird.target.position + Vector3.up *.3f;
                //after 1 second set the bird to welcoming
                bird.SetNewSettings(welcomingSettings);
                break;
            case BirdState.GoToLanding:
                bird.currentWaypoint = bird.landingSpot.position;
                bird.SetNewSettings(goToLandingSettings);

                break;
            case BirdState.Landing:
                bird.SwitchAnimationState(birdState);

                bird.SetNewSettings(LandedSettings);
                //Invoke("TakeOff",6);
                break;
            case BirdState.TakeOff:
                bird.SwitchAnimationState(birdState);
                SwitchState(BirdState.Hunting);
                break;
            case BirdState.Exiting:
                break;
            case BirdState.Catching:
                bird.prey.gameObject.SetActive(true);
                break;

            default:
                break;
        }
        stateText.text = birdState.ToString();

        currentState = birdState;
    }


    // Update is called once per frame
    void Update()
    {
        
        if(OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, controllerR) || Keyboard.current[Key.W].wasPressedThisFrame)
        {
            SwitchState(BirdState.Welcoming);
        }
        if(OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, controllerL)|| Keyboard.current[Key.H].wasPressedThisFrame)
        {
            if (currentState == BirdState.Landing)
            {
                bird.anim.SetTrigger("TakeOff");
                SwitchState(BirdState.TakeOff);
            }
            else
            {
                SwitchState(BirdState.Hunting);
            }
        }
        if(OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, controllerR)|| Keyboard.current[Key.L].wasPressedThisFrame)
        {
            SwitchState(BirdState.GoToLanding);
        }    
        if(OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, controllerL)|| Keyboard.current[Key.T].wasPressedThisFrame)
        {
            bird.anim.SetTrigger("TakeOff");
            SwitchState(BirdState.TakeOff);
        }
        if(Keyboard.current[Key.C].wasPressedThisFrame)
        {
            SwitchState(BirdState.Catching);
        }
    }
    
    void SetBirdSettings()
    {
        //**creating each bird setting for us to use. we can add custom speed, waypoint logic etc
        huntingSettings = new BirdSettings(bird.turnAngleIntensity, bird.waypointRadius, bird.waypointProximity, bird.speed, bird.turnSpeed);
        catchingSettings = new BirdSettings(bird.turnAngleIntensity, bird.waypointRadius, bird.waypointProximity, bird.speed, bird.turnSpeed*3f);
        welcomingSettings = new BirdSettings(0f, bird.waypointRadius, 1, bird.speed * 1.3f, bird.turnSpeed *3);
        goToLandingSettings = new BirdSettings(0f, bird.waypointRadius, .3f, bird.speed * 1.3f, bird.turnSpeed *15);
        exitingSettings = new BirdSettings(0f, bird.waypointRadius, 0, bird.speed * 1.3f, bird.turnSpeed);
        LandedSettings = new BirdSettings(0f, bird.waypointRadius, 0, 0f, bird.turnSpeed);
    }
}
