using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BirdMovement : MonoBehaviour
{
    public BirdStateChanger birdState;
    
    // Flying
    public float turnSpeed;
    public float speed;
    public Transform target;
    public float turnAngleIntensity;
    
    //Rotation
    private Vector3 direction;
    private Quaternion rotationGoal;
    private float currentTurnSpeed;
    
    //Orbiting
    public float orbitSpeed;
    public float minOrbitRadius;
    private Vector3 previousPos;

    // Waypoints
    public float waypointRadius;
    public float waypointProximity;
    public float minHeight, maxHeight;
    public Transform landingSpot;

    [HideInInspector]
    public Vector3 currentWaypoint;

    // Animation
    public Transform birdhead;
    public Animator anim;
    private bool gliding;
    
    // Testing
    public GameObject testcube;
    public GameObject currentCube;
    

    private float flappngRate = 1;
    private float originalFlappngRate = 1;
    
    private float originalGlidingRate = 5;
    private float glidingRate = 5;

    void Start()
    {
        birdState = GetComponent<BirdStateChanger>();
        anim = GetComponent<Animator>();
        // Spawning a new waypoint every 10 seconds
        //InvokeRepeating("FindNewWaypoint", 1f, 10f);
        // Spawn a new waypoint at the beginning of the game
        FindNewWaypoint();
        //StartCoroutine("Welcome");
        
        StartCoroutine("RandomFlapping");
        SwitchAnimation("Glide");

    }

     IEnumerator RandomFlapping()
     {
         yield return new WaitForSeconds(Random.Range(glidingRate, glidingRate*2.2f)); 
         SwitchAnimation("Flap"); 
         
         yield return new WaitForSeconds(Random.Range(flappngRate, flappngRate*2)); 
         SwitchAnimation("Glide"); 
         StartCoroutine("RandomFlapping");
     }


     
    void SwitchAnimation(string triggerName)
    {
        if (birdState.currentState == BirdStateChanger.BirdState.Landing) return;
        if (anim.GetBool(triggerName))
        {
            return;
        }
        anim.SetTrigger(triggerName);
    }

    void FindNewWaypoint()
    {
        // Pick a random point in a sphere of 1
        Vector3 randomPos = Random.insideUnitSphere;

        // Multiply the width and length * waypointRadius
        randomPos.x *= waypointRadius;
        randomPos.z *= waypointRadius;
        randomPos.y = Random.Range(minHeight, maxHeight);

        // Multiply the height * a random number between minHeight and maxHeight
        currentWaypoint = target.position + randomPos;

        // For testing spawn a cube at the new waypoint
        if( currentCube !=null) Destroy(currentCube);
        currentCube = Instantiate(testcube, currentWaypoint, Quaternion.identity);

    }

    // Update is called once per frame
    void Update()
    {
       // Debug.DrawRay(transform.position, direction *10f);
        //Debug.DrawRay(transform.position, transform.forward *10f, Color.blue,Time.deltaTime);
        // Check which State we are in 
        switch (birdState.currentState)
        {
            case BirdStateChanger.BirdState.Hunting:
                BasicFlying();

                break;
            case BirdStateChanger.BirdState.GoToLanding:
                BasicFlying();

                break;

            case BirdStateChanger.BirdState.Landing:
                transform.position = Vector3.MoveTowards(transform.position, landingSpot.position + Vector3.up * .1f, .4f * Time.deltaTime);
                
                FaceTowardMovement();
                ResetXAngle();

                break;
            case BirdStateChanger.BirdState.Welcoming:
                BasicFlying();
                break;

            case BirdStateChanger.BirdState.Orbiting:
                OrbitFlying();
                
                break;
            case BirdStateChanger.BirdState.Exiting:

                break;
            case BirdStateChanger.BirdState.TakeOff:
                BasicFlying();

                break;
            default:
                break;
        }
        
        // work on this later - birdhead.LookAt(target);
    }

    private void ResetXAngle()
    {
        Vector3 targetRotation = transform.eulerAngles;
        //move x rotation until it's 0
        if (targetRotation.x > 230 && targetRotation.x < 360)
        {
            targetRotation.x += 20 * Time.deltaTime;
            transform.eulerAngles = targetRotation;
        }
        else
        {
            targetRotation.x = 0f;
            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, targetRotation, .1f);
        }
    }

    public void SetNewSettings(BirdStateChanger.BirdSettings newSettings)
    {
        //go through each bird movement variable and switch it to the new setting
        turnAngleIntensity = newSettings.turnAngleIntensity;
        waypointRadius = newSettings.waypointRadius;
        waypointProximity = newSettings.waypointProximity;
        speed =newSettings.speed;
        turnSpeed = newSettings.turnSpeed;
    }

    private void Tilt()
    {
        // Compare the forward angle of the bird and the direction of the waypoint
        float angle = Vector3.SignedAngle(transform.forward, direction, Vector3.up);
        angle *= turnAngleIntensity;
        // Angle bird towards waypoint
        if(Mathf.Abs(angle) > 14) SwitchAnimation("Glide");

        transform.Rotate(Vector3.forward, angle);
    }

    private void DistanceCheck()
    {
        // Checking distance between waypoint and bird position, if it is less than distance find a new spot
        if (!(Vector3.Distance(currentWaypoint, transform.position) < waypointProximity)) return;
        
        
        if (birdState.currentState == BirdStateChanger.BirdState.Welcoming)
        {
            birdState.SwitchState(BirdStateChanger.BirdState.Orbiting);
        }
        else if (birdState.currentState == BirdStateChanger.BirdState.GoToLanding)
        {
            birdState.SwitchState(BirdStateChanger.BirdState.Landing);
        }   
        else
        {
            FindNewWaypoint();
        }
    }

    private void BasicFlying()
    {
        ForwardMovement();
        DistanceCheck();
        Tilt();
    }

    private void OrbitFlying()
    {
        FaceTowardMovement();
        Tilt();
    }

    private void OrbitRotation()
    {
        Vector3 dir = transform.position - previousPos;
        rotationGoal = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotationGoal, turnSpeed);

        //transform.rotation = Quaternion.LookRotation(dir.normalized);
        previousPos = transform.position;
    }

    void FaceTowardMovement()
    {
        if (Vector3.Distance(target.position, transform.position) > minOrbitRadius)
        {
            transform.RotateAround(target.position, Vector3.up, orbitSpeed * Time.deltaTime);
            OrbitRotation();
        }
        else
        {   
            transform.Translate(Vector3.forward* speed * Time.deltaTime);
        }
    }
    
    private void ForwardMovement()
    {
        // Move forward
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // Finding waypoint direction
        direction = (currentWaypoint - transform.position).normalized;
        rotationGoal = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotationGoal, turnSpeed);
        DrawDirectionRays();


        FlapWingCheck();
    }

    private void DrawDirectionRays()
    {
        Debug.DrawRay(transform.position, Quaternion.Lerp(transform.rotation, rotationGoal, .1f) * Vector3.forward * 10f, Color.green, Time.deltaTime);
        Debug.DrawRay(transform.position, Quaternion.Lerp(transform.rotation, rotationGoal, .2f) * Vector3.forward * 10f, Color.green, Time.deltaTime);
        Debug.DrawRay(transform.position, Quaternion.Lerp(transform.rotation, rotationGoal, .3f) * Vector3.forward * 10f, Color.green, Time.deltaTime);
        Debug.DrawRay(transform.position, Quaternion.Lerp(transform.rotation, rotationGoal, .5f) * Vector3.forward * 10f, Color.green, Time.deltaTime);
        Debug.DrawRay(transform.position, Quaternion.Lerp(transform.rotation, rotationGoal, .7f) * Vector3.forward * 10f, Color.green, Time.deltaTime);
        Debug.DrawRay(transform.position, Quaternion.Lerp(transform.rotation, rotationGoal, .8f) * Vector3.forward * 10f, Color.green, Time.deltaTime);
        Debug.DrawRay(transform.position, Quaternion.Lerp(transform.rotation, rotationGoal, .9f) * Vector3.forward * 10f, Color.green, Time.deltaTime);
    }


    private void FlapWingCheck()
    {
        if (transform.forward.y < -.05f)
        {
            flappngRate = originalFlappngRate * .1f;
            glidingRate = originalGlidingRate * 2;

            SwitchAnimation("Glide");
            anim.ResetTrigger("Flap");
        }
        else if (transform.forward.y > .1f)
        {
            flappngRate = originalFlappngRate * 2;
            glidingRate = originalGlidingRate * .1f;

            SwitchAnimation("Flap");
            anim.ResetTrigger("Glide");

        }
        else
        {
            flappngRate = originalFlappngRate;
            glidingRate = originalGlidingRate;
        }
    }

    public void SwitchAnimationState(BirdStateChanger.BirdState newState)
    {
        switch (newState)
        {
            case BirdStateChanger.BirdState.TakeOff:
                anim.SetBool("OnGround",false);
                SwitchAnimation("Flap");
                SwitchAnimation("TakeOff");

                break;
            case BirdStateChanger.BirdState.Landing:
                
                anim.SetBool("OnGround",true);
                SwitchAnimation("Hover");
                break;
        }

    }
}

