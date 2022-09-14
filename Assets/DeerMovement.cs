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
        anim = GetComponent<Animator>();
        waypoint = firstLocationGoal.position;
        SetAnimalSettings();
        UpdateAnimalSettings(goToTargetSettings);
    }

    void SetAnimalSettings()
    {
        wanderingSettings = new AnimalSettings( waypointRadius, waypointProximity, speed, turnSpeed);
        goToTargetSettings = new AnimalSettings( waypointRadius, waypointProximity * .1f, speed, turnSpeed *3f);

    }

    void UpdateAnimalSettings(AnimalSettings newSetting)
    {
        currentSettings = newSetting;
        turnSpeed = newSetting.turnSpeed;
        waypointRadius = newSetting.waypointRadius;
        speed = newSetting.speed;
        waypointProximity = newSetting.waypointProximity;
    }


    void SwitchState(AnimalStates newState)
    {
        if (newState == currentState) return;

        currentState = newState;

        SwitchAnimations(currentState);
        
        switch (currentState)
        {
            
        }
    }

    private void SwitchAnimations(AnimalStates newState)
    {
        switch (newState)
        {
            case AnimalStates.LieDown:
                anim.SetBool("LyingDown", true);
                StartCoroutine(nameof(WaitAndStartFeedback));

                break;
            case AnimalStates.StopAndLook:
                anim.SetTrigger("StopAndLook");
                StartCoroutine(WaitAndSwitchState(AnimalStates.Eating, 1.4f));
                break;
            case AnimalStates.Eating:
                anim.SetBool("Eating",true);
                StartCoroutine(WaitAndSwitchState(AnimalStates.LieDown, 5f));
                break;
            case AnimalStates.Trotting:
                anim.SetBool("Eating",false);
                anim.SetBool("Walking",false);

                anim.SetBool("Trotting",true);
                break;
        }    
    }

    IEnumerator WaitAndSwitchState(AnimalStates nextState, float time)
    {
        yield return new WaitForSeconds(time);
        SwitchState(nextState);
    }    
    
    IEnumerator WaitAndStartFeedback()
    {
        yield return new WaitForSeconds(10f);
        if(introSequence) FindObjectOfType<FeedbackLogic>().StartFeedback();
    }
    
    // Update is called once per frame
    void Update()
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
                    SwitchState(AnimalStates.StopAndLook);
                }
                else
                {
                    FindNewWaypoint();
                }
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
