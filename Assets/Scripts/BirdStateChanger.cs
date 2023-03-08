using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
using UnityEngine.InputSystem;

public class BirdStateChanger : MonoBehaviour
{
    OVRInput.Controller controllerR = OVRInput.Controller.RTouch;
    OVRInput.Controller controllerL = OVRInput.Controller.LTouch;

    public TextMeshPro stateText;
    public GameObject ghostHand;

    public bool customControlsUnlocked;
    public AudioSource birdWind;

    public StumpBehavior stump;
    
    public TextMeshPro takeOffText;

    public GameObject windForceField;


    private BirdMovement bird;
    private static readonly int TakeOff = Animator.StringToHash("TakeOff");

    private StoryParts story;
    //private
    bool mistWindSceneActivated;

    public Transform testCube;

    private Wind wind;
    bool waitingForMistWind;
    // States
    public enum BirdState
    {
        Hunting, Welcoming, Exiting, Orbiting, GoToLanding, Landed,
        Landing,
        TakeOff,
        Diving,
        Eating,
        FacingPlayer,
        Flapping,
        FlyingAway
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

    private BirdSettings flappingSettings;
    private BirdSettings huntingSettings; 
    private BirdSettings welcomingSettings; 
    private BirdSettings goToLandingSettings; 
    private BirdSettings LandedSettings; 
    private BirdSettings exitingSettings;
    private BirdSettings divingSettings;

    public BirdStateChanger(bool customControlsUnlocked)
    {
        this.customControlsUnlocked = customControlsUnlocked;
    }

    private void Awake()
    {

        wind = FindObjectOfType<Wind>();
        story = FindObjectOfType<StoryParts>();
        bird = GetComponent<BirdMovement>();
        SetBirdSettings();
    }

    public void UpdateSettings(BirdStateChanger.BirdSettings newSettings)
    {
        //go through each bird movement variable and switch it to the new setting
        bird.turnAngleIntensity = newSettings.turnAngleIntensity;
        bird.waypointRadius = newSettings.waypointRadius;
        bird.waypointProximity = newSettings.waypointProximity;
        bird.speed = newSettings.speed;
        bird.turnSpeed = newSettings.turnSpeed;
    }
    
    // Change the bird from flying to meeting player to leaving
    public void SwitchState(BirdState birdState)
    {
        if (currentState == birdState) return;

        if (currentState == BirdState.Diving
            && birdState != BirdStateChanger.BirdState.GoToLanding)
        {
            if (birdState != BirdStateChanger.BirdState.FacingPlayer)
            {
                print("Didn't switch because current state " + currentState + " new state is " + birdState);
                return;
            }

        }

        print("Switching state " + birdState);

        switch (birdState)
        {
            case BirdState.Orbiting:

                if (!story.firstWelcomeDone)
                {
                    StartCoroutine(nameof(ContinueIntroSequence));
                }
                else
                {
                    if (mistWindSceneActivated) return;
                    StartCoroutine(nameof(WaitAndHunt));

                }
                break;
            
            case BirdState.FacingPlayer:
                bird.anim.SetBool("Eating",false);
                bird.waypointProximity = 3;
                bird.speed = 1.2f;
                if(bird.prey != null)
                {
                    bird.prey.gameObject.GetComponent<MeshRenderer>().enabled = false;
                }
                break;

            case BirdState.Flapping:
                UpdateSettings(flappingSettings);
                bird.SwitchAnimationState(birdState);
                bird.anim.ResetTrigger("Glide");
                bird.anim.ResetTrigger("Fly");
                bird.birdAudio.PlaySound("strongWind");
                Invoke("PlayScreech", 2);
                Invoke("FlyAway", 12);

                wind.StartTornado();

                break;

            case BirdState.FlyingAway:
                bird.currentWaypoint = bird.player.position + Vector3.left * 20 + Vector3.up * 30 + Vector3.forward *20;
                testCube.position = bird.currentWaypoint;
                IEnumerator resetSky = wind.ResetSky();
                
                StartCoroutine(resetSky);
                bird.anim.SetBool("Flapping", false);

                UpdateSettings(huntingSettings);
                birdWind.Play();

                IEnumerator WaitAndHideBird = bird.ShrinkAndDeactivate();
                StartCoroutine(WaitAndHideBird);

                break;
            
            case BirdState.Hunting:
                if (mistWindSceneActivated || bird.grabbedFish) return;

                bird.anim.SetBool("Flapping", false);

                UpdateSettings(huntingSettings);
                birdWind.Play();
                break;

            case BirdState.Welcoming:
                if (mistWindSceneActivated) SwitchState(BirdState.Hunting);
                if (bird.grabbedFish) return;

                bird.anim.SetBool("Eating", false);

                birdWind.Play();

                bird.currentWaypoint = bird.player.position + Vector3.up *.5f;
                bird.SwitchAnimationState(birdState);

                //after 1 second set the bird to welcoming
                UpdateSettings(welcomingSettings);
                break;

            case BirdState.GoToLanding:
                if (mistWindSceneActivated) SwitchState(BirdState.Hunting);
                bird.SwitchAnimationState(birdState);

                UpdateSettings(goToLandingSettings);

                break;
            case BirdState.Landing:
                if (mistWindSceneActivated) SwitchState(BirdState.Hunting);
                birdWind.Stop();
                bird.SwitchAnimationState(birdState);
                UpdateSettings(LandedSettings);

                if (bird.grabbedFish)
                {
                    SwitchState(BirdState.Eating);
                    IEnumerator facePlayer = bird.FacePlayer();
                    StartCoroutine(facePlayer);
                    if (bird.fishCaught == 1)
                    {
                        ghostHand.SetActive(true);
                        ghostHand.transform.SetParent(null);
                    }
                }
        
                if (bird.landingSpot == bird.handLandingSpot)
                {
                    FindObjectOfType<GoogleSheets>().AddEventData("Eagle on Hand", SystemInfo.deviceUniqueIdentifier);
                    new Cognitive3D.CustomEvent("Eagle on Hand");
                    FindObjectOfType<ControlUIManager>().ToggleEagleControlUI(true);
                }

                break;
            
            case BirdState.TakeOff:
                if(mistWindSceneActivated) SwitchState(BirdState.Hunting);

                bird.SwitchAnimationState(birdState);
                SwitchState(BirdState.Hunting);
                break;
            
            case BirdState.Exiting:
                break;
            
            case BirdState.Diving:
                if (mistWindSceneActivated) SwitchState(BirdState.Hunting);
                if (bird.grabbedFish) return;
                if (currentState == BirdState.Landing) bird.anim.SetTrigger("TakeOff");

                    bird.SwitchAnimationState(birdState);
                bird.prey.gameObject.SetActive(true);
                UpdateSettings(divingSettings);

                //StartCoroutine("ResetToHunting");
                break;
            
            case BirdState.Eating:
                birdWind.Stop();
                bird.SwitchAnimationState(birdState);
                break;
        }

        currentState = birdState;
        FindObjectOfType<GoogleSheets>().AddEventData("Bird State " + currentState, SystemInfo.deviceUniqueIdentifier);
        new Cognitive3D.CustomEvent($"Bird State {currentState}");
        
    }

    IEnumerator ContinueIntroSequence()
    {
        story.firstWelcomeDone = true;
        yield return new WaitForSeconds(12);
        SwitchState(BirdState.Hunting);
        yield return new WaitForSeconds(16);
        SwitchState(BirdState.Welcoming);
    }
    
    

    IEnumerator WaitAndHunt()
    {
        yield return new WaitForSeconds(20);
        if (currentState == BirdState.Diving || currentState == BirdState.GoToLanding) yield break;
        
        SwitchState(BirdState.Hunting);
    }

    
    void FlyAway()
    {
        SwitchState(BirdState.FlyingAway);
    }
    void PlayScreech()
    {
        bird.birdAudio.PlaySound("birdScream");

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

            //AddFish
            if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch) || Keyboard.current[Key.S].wasPressedThisFrame)
            {
                FindObjectOfType<GoogleSheets>().AddEventData("Added more fish", SystemInfo.deviceUniqueIdentifier);
                new Cognitive3D.CustomEvent("Added more fish");
                if (stump.fishSystemOn)
                {
                    stump.SpawnMorePrey();
                }
            }


            if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.LTouch) || Keyboard.current[Key.S].wasPressedThisFrame)
            {
                FindObjectOfType<GoogleSheets>().AddEventData("SlowMo Pressed", SystemInfo.deviceUniqueIdentifier);
                new Cognitive3D.CustomEvent("SlowMo Pressed");
                bird.ToggleSloMo(true);
            }

            if (OVRInput.GetUp(OVRInput.Button.One, OVRInput.Controller.LTouch) || Keyboard.current[Key.S].wasPressedThisFrame)
            {
                bird.ToggleSloMo(false);
            }

            //Fly By
            if(OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch) || Keyboard.current[Key.W].wasPressedThisFrame)
            {
                stateText.text = "Fly By";

                SwitchState(BirdState.Welcoming);
            }
            
            if(OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.LTouch) || Keyboard.current[Key.H].wasPressedThisFrame)
            {
                if (currentState != BirdState.Landing)
                {
                    stateText.text = "Explore";
                    SwitchState(BirdState.Hunting);
                }

            }

            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch)  || Keyboard.current[Key.T].wasPressedThisFrame)
            {

                if (currentState == BirdState.Landed || currentState == BirdState.Landing)
                {

                    bird.anim.SetTrigger(TakeOff);
                    SwitchState(BirdState.TakeOff);

                    stateText.text = "Explore";
                    takeOffText.text = "Land";
                    
                    StartCoroutine(nameof(WaitAndClearFog));
                }
                else
                {
                    bird.landingSpot = GetComponent<BirdMovement>().handLandingSpot;
                    SwitchState(BirdState.GoToLanding);

                    takeOffText.text = "Take Off";
                    stateText.text = "Land";
                }
            }
            
        }
        
    }


    IEnumerator WaitAndClearFog()
    {
        print("Waiting and clearing fog. mistWindSceneActivated " + mistWindSceneActivated);
        if (mistWindSceneActivated || waitingForMistWind) yield break;
        waitingForMistWind = true;


        yield return new WaitForSeconds(60);


        print("60s econds passed");
        FindObjectOfType<ControlUIManager>().ToggleEagleControlUI(false);
        customControlsUnlocked = false;
        
        switch (currentState)
        {
            case BirdState.GoToLanding:
            case BirdState.Diving:
            case BirdState.Welcoming:
            case BirdState.Orbiting:
                print("preparing bird for facing player switching " + currentState + " to hunting ");
                SwitchState(BirdState.Hunting);
                break;
            case BirdState.Landed:
            case BirdState.Landing:
            case BirdState.Eating:
                print("preparing bird for facing player switching " + currentState + " to takeoff ");

                SwitchState(BirdState.TakeOff);
                break;
        }

        mistWindSceneActivated = true;

        print("10 seconds to face player");
        yield return new WaitForSeconds(10);

        print("about to face player");

        SwitchState(BirdState.FacingPlayer);
    }

    void SetBirdSettings()
    {
        //**creating each bird setting for us to use. we can add custom speed, waypoint logic etc
        flappingSettings = new BirdSettings(bird.turnAngleIntensity, bird.waypointRadius, bird.waypointProximity, 0, bird.turnSpeed);
        huntingSettings = new BirdSettings(bird.turnAngleIntensity, bird.waypointRadius, bird.waypointProximity, bird.speed, bird.turnSpeed);
        divingSettings = new BirdSettings(bird.turnAngleIntensity, bird.waypointRadius*3f, 1f, bird.speed, bird.turnSpeed *1.5f);
        welcomingSettings = new BirdSettings(0f, bird.waypointRadius, 1.2f, bird.speed * 1.3f, bird.turnSpeed * 1.5f);
        goToLandingSettings = new BirdSettings(0f, bird.waypointRadius,  1f, bird.speed * 1.3f, bird.turnSpeed );
        exitingSettings = new BirdSettings(0f, bird.waypointRadius, 0, bird.speed * 1.3f, bird.turnSpeed);
        LandedSettings = new BirdSettings(0f, bird.waypointRadius, 0, 0f, bird.turnSpeed);
    }
}
