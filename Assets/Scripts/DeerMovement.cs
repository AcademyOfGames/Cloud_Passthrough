using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class DeerMovement : MonoBehaviour
{
    public float speed;
    public float turnSpeed;
    public Transform firstLocationGoal;
    private Vector3 waypoint;
    private bool introSequence = true;
    private GameObject currentCube;
    public GameObject testcube;
    public Transform sittingUnderTreeTransform;

    public enum AnimalStates { Walking, Eating, StopAndLook,
        Trotting, LieDown
    }

    public AnimalStates currentState = AnimalStates.Walking;

    private Animator anim;

    public float waypointProximity = 3f;

    public Transform target;
    public float waypointRadius;

    AnimalSettings goToTargetSettings;
    AnimalSettings wanderingSettings;
    AnimalSettings eatingSettings;
    AnimalSettings runningSettings;


    public class AnimalSettings
    {
        public float waypointRadius;
        public float waypointProximity;
        public float speed;
        public float turnSpeed;

        //** This is called a constructor, when you write new BirdSettings(...) in your code you can create a new BirdSettings object with specific data
        public AnimalSettings( float wRadius, float wProximity, float vSpeed, float tSpeed)
        {
            waypointRadius = wRadius;
            waypointProximity = wProximity;
            speed = vSpeed;
            turnSpeed = tSpeed;
        }
    }


    public AnimalSettings currentSettings;
    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<GoogleSheets>().AddEventData("Deer showed up", SystemInfo.deviceUniqueIdentifier);
        new Cognitive3D.CustomEvent("Deer showed up");
        anim = GetComponent<Animator>();
        waypoint = firstLocationGoal.position;
        SetAnimalSettings();
        UpdateAnimalSettings(currentState);
    }

    void SetAnimalSettings()
    {
        wanderingSettings = new AnimalSettings( waypointRadius, waypointProximity, speed, turnSpeed);
        goToTargetSettings = new AnimalSettings( waypointRadius, waypointProximity , speed, turnSpeed *3f);
        eatingSettings = new AnimalSettings( waypointRadius, waypointProximity , 0, 0);
        runningSettings = new AnimalSettings( waypointRadius, waypointProximity , speed*3f, turnSpeed *3f);

    }

    void UpdateAnimalSettings(AnimalStates newState)
    {
        AnimalSettings settings = wanderingSettings;
        switch (newState)
        {
            case AnimalStates.LieDown:
                settings = eatingSettings;
                break;
            case AnimalStates.StopAndLook:
                settings = eatingSettings;
                break;
            case AnimalStates.Eating:
                settings = eatingSettings;
                break;
            case AnimalStates.Walking:
                settings = wanderingSettings;
                break;
            case AnimalStates.Trotting:
                settings = runningSettings;
                break; 
        }
        

        currentSettings = settings;
        turnSpeed = settings.turnSpeed;
        waypointRadius = settings.waypointRadius;
        speed = settings.speed;
        waypointProximity = settings.waypointProximity;
    }

    

    public void SwitchState(AnimalStates newState)
    {
        if (newState == currentState) return;

        switch (newState)
        {
            case AnimalStates.LieDown:
                anim.SetBool("LyingDown", true);
                break;
            
            case AnimalStates.StopAndLook:
                anim.SetTrigger("StopAndLook");
                StartCoroutine(WaitAndSwitchState(AnimalStates.Trotting,1f));
                break;
            
            case AnimalStates.Eating:
                anim.SetBool("Eating",true);
                StartCoroutine(WaitAndSwitchState(AnimalStates.StopAndLook, 10.5f));

                Invoke("WolfHowl",10);
                break;
            
            case AnimalStates.Trotting:
                anim.SetBool("Eating",false);
                anim.SetBool("Walking",false);
                anim.SetBool("Trotting",true);
                break;
        }

        currentState = newState;
        UpdateAnimalSettings(currentState);
    }

    void WolfHowl()
    {
        FindObjectOfType<WolfLogic>().PlayAudio("DistantHowl");
    }
    IEnumerator WaitAndSwitchState(AnimalStates nextState, float time)
    {
        yield return new WaitForSeconds(time);
        SwitchState(nextState);
    }    
    
    
    // Update is called once per frame
    void FixedUpdate()
    {
        switch (currentState)
        {
            case AnimalStates.Walking:
                Quaternion rotationGoal = Quaternion.LookRotation(waypoint - transform.position );

                transform.rotation = Quaternion.Slerp(transform.rotation, rotationGoal, turnSpeed * .001f);
                //transform.LookAt(waypoint.position);
                transform.Translate(Vector3.forward * speed);
            break;
            case AnimalStates.Trotting:
                //transform.LookAt(waypoint.position);
                transform.Translate(Vector3.forward * speed *2f);
                break;
        }

        DistanceCheck();
    }

    private void DistanceCheck()
    {
        float waypointDistance = Vector3.Distance(waypoint, transform.position);
        
        if(waypointDistance < waypointProximity)
        {
            if (currentState == AnimalStates.Walking)
            {

                if (introSequence)
                {
                    SwitchState(AnimalStates.Eating);
                }
                else
                {
                    FindNewWaypoint();
                }
            }
            else
            {
                print("isnot walking");
            }
        }
    }

    private void FindNewWaypoint()
    {
        // Pick a random point in a sphere of 1
        Vector3 randomPos = Random.insideUnitSphere;

        // Multiply the width and length * waypointRadius
        randomPos.x *= waypointRadius;
        randomPos.z *= waypointRadius;
        randomPos.y = transform.position.y;
        // Multiply the height * a random number between minHeight and maxHeight
        waypoint = target.position + randomPos;

        // For testing spawn a cube at the new waypoint
        if( currentCube !=null) Destroy(currentCube);

        if (testcube != null)
        {
            currentCube = Instantiate(testcube, waypoint, Quaternion.identity);

            currentCube.transform.localScale *= .5f;        
        }
    }


}
