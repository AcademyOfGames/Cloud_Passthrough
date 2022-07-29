using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
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
    public Transform handLandingSpot;
    public Transform branchLandingSpot;

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

    public Transform prey;
    public float maxSpeed;

    void Start()
    {
        birdState = GetComponent<BirdStateChanger>();
        anim = GetComponent<Animator>();

        // Spawn a new waypoint at the beginning of the game
        FindNewWaypoint();
        
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
        float distance = Vector2.Distance(new Vector2(randomPos.x, randomPos.z), new Vector2(target.position.x,target.position.z));

        randomPos.y = Random.Range(minHeight, maxHeight+distance*.4f);
        // Multiply the height * a random number between minHeight and maxHeight
        currentWaypoint = target.position + randomPos;

        print("Finding waypoint");
        // For testing spawn a cube at the new waypoint
        if( currentCube !=null) Destroy(currentCube);
        currentCube = Instantiate(testcube, currentWaypoint, Quaternion.identity);
        currentCube.transform.localScale *= .5f;

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("floor"))
        {
            transform.Translate(Vector3.up *.05f);
        }
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
            case BirdStateChanger.BirdState.Diving:
                currentWaypoint = prey.position;
                //currentWaypoint.y -= Vector3.Distance(prey.position, transform.position);
                BasicFlying();
                if (speed < maxSpeed)
                {
                    speed *= 1.01f;

                }
                turnSpeed *= 1.01f;
                
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
    public void UpdateSettings(BirdStateChanger.BirdSettings newSettings)
    {
        //go through each bird movement variable and switch it to the new setting
        turnAngleIntensity = newSettings.turnAngleIntensity;
        waypointRadius = newSettings.waypointRadius;
        waypointProximity = newSettings.waypointProximity;
        speed =newSettings.speed;
        turnSpeed = newSettings.turnSpeed;
    }
    
    private void DistanceCheck()
    {
        // Checking distance between waypoint and bird position, if it is less than distance find a new spot
        if (!(Vector3.Distance(currentWaypoint, transform.position) < waypointProximity)) return;


        //decide what to do when it reaches a state
        switch (birdState.currentState)
        {
            case BirdStateChanger.BirdState.Welcoming:
                birdState.SwitchState(BirdStateChanger.BirdState.Orbiting);
                break;
            case BirdStateChanger.BirdState.GoToLanding:
                birdState.SwitchState(BirdStateChanger.BirdState.Landing);
                break;
            case BirdStateChanger.BirdState.Diving:
                SwitchAnimation("Catching");
                break;
            default:
                FindNewWaypoint();
break;
        }
        
        if (birdState.currentState == BirdStateChanger.BirdState.Welcoming)
        {
        }
        else if (birdState.currentState == BirdStateChanger.BirdState.GoToLanding)
        {
        }   
        else
        {
        }
    }

    private void BasicFlying()
    {
        ForwardMovement();
        DistanceCheck();
       // Tilt();
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
    private void Tilt()
    {
        // Compare the forward angle of the bird and the direction of the waypoint
        float angle = Vector3.SignedAngle(transform.forward, direction, Vector3.up);
        angle *= turnAngleIntensity;
        
        
        // Angle bird towards waypoint
        if(Mathf.Abs(angle) > 14) SwitchAnimation("Glide");
        
        transform.Rotate(Vector3.forward,angle);
        
    }
    private float TiltWithReturn()
    {
        // Compare the forward angle of the bird and the direction of the waypoint
        float angle = Vector3.SignedAngle(transform.forward, direction, Vector3.up);
        angle *= turnAngleIntensity;
        float tempAngle = transform.eulerAngles.z;

        if (tempAngle > 180)
        {
            tempAngle -= 360;
        }
        angle = Mathf.Lerp(tempAngle, angle, .004f);
        if(Mathf.Abs(angle) > 20) SwitchAnimation("Flap");

        //print("Going from " + transform.eulerAngles.z + " to " + angle);
        // Angle bird towards waypoint

        return angle;

    }
    private void ForwardMovement()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        
        direction = (currentWaypoint - transform.position).normalized;
        Vector3 eulerRotationGoal = Quaternion.LookRotation(direction).eulerAngles;
        
        Quaternion flatVersion;
 
        flatVersion =
            Quaternion.Euler(
                eulerRotationGoal.x,
                eulerRotationGoal.y,
                transform.eulerAngles.z
            );
 
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            flatVersion,
            turnSpeed);
        
        
        Vector3 tilt = transform.eulerAngles;
        tilt.z = TiltWithReturn();
        transform.rotation = Quaternion.Euler(tilt);
        
        FlapWingCheck();
    }
    
    private void ForwardMovement2()
    {
        // Move forward
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // Finding waypoint direction
        direction = (currentWaypoint - transform.position).normalized;
        Vector3 eulerRotationGoal = Quaternion.LookRotation(direction).eulerAngles;

        Vector3 tempRotation = transform.eulerAngles;
        DrawDirectionRays();

        print("Blending between " + tempRotation.x + " to goal of " + rotationGoal.eulerAngles.x);
        if (eulerRotationGoal.x > 180)
        {
            eulerRotationGoal.x -= 360f;
        }
        tempRotation.x = Mathf.Lerp(tempRotation.x, rotationGoal.eulerAngles.x, turnSpeed);
        //tempRotation.y = Mathf.Lerp(tempRotation.y, rotationGoal.eulerAngles.y, turnSpeed);
        transform.eulerAngles = tempRotation;

/*
 
        Quaternion rotationWithoutZ = transform.rotation;
        rotationWithoutZ.z = 0;

        rotationWithoutZ = Quaternion.Lerp(rotationWithoutZ, rotationGoal, turnSpeed);

        transform.rotation = new Quaternion(rotationWithoutZ.x, rotationWithoutZ.y, transform.rotation.z,rotationWithoutZ.w);
        */
        
        
        //transform.rotation.z = Quaternion.Lerp(rotationWithoutZ, rotationGoal, turnSpeed);

      //  float tempEulerX = Mathf.Lerp(transform.eulerAngles.x, rotationGoal.eulerAngles.x, .5f);
       // transform.eulerAngles = new Vector3(tempEulerX, transform.eulerAngles.y, transform.eulerAngles.z);



    }

    private void DrawDirectionRays()
    {
        Vector3 tempRotation = transform.eulerAngles;
        
        float newRotX = Mathf.Lerp(tempRotation.x, rotationGoal.eulerAngles.x, .1f);
        Vector3 newRot = tempRotation;
        newRot.x = newRotX;
        Debug.DrawRay(transform.position, Quaternion.Euler(newRot) *transform.forward * 10f, Color.green, Time.deltaTime);

        newRotX = Mathf.Lerp(tempRotation.x, rotationGoal.eulerAngles.x, .4f);
        newRot = tempRotation;
        newRot.x = newRotX;
        Debug.DrawRay(transform.position, Quaternion.Euler(newRot) *transform.forward * 10f, Color.green, Time.deltaTime);
        
        Debug.DrawRay(transform.position, Quaternion.Euler(rotationGoal.eulerAngles)*transform.forward * 10f, Color.blue, Time.deltaTime);
        Debug.DrawRay(transform.position, Quaternion.Euler(transform.forward) * transform.forward * 10f, Color.red, Time.deltaTime);

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
            case BirdStateChanger.BirdState.Diving:
                SwitchAnimation("Diving");
                break;
        }

    }
}

