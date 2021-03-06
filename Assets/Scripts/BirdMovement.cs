using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class BirdMovement : MonoBehaviour
{
    public BirdStateChanger birdState;
    //Custom behavior
    private bool introSequenceDone;
    public GameObject fishBucket;
    public GameObject[] customControlsUI;
    
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
    private static readonly int OnGround = Animator.StringToHash("OnGround");
    private static readonly int Eating = Animator.StringToHash("Eating");
    private static readonly int Flap = Animator.StringToHash("Flap");
    private static readonly int Glide = Animator.StringToHash("Glide");
    public bool grabbedFish { get; set; }
    public int fishCaught { get; set; }
    private BirdAudioManager birdAudio;

    void Start()
    {
        birdAudio = GetComponent<BirdAudioManager>();
        fishCaught = 0;
        birdState = GetComponent<BirdStateChanger>();
        anim = GetComponent<Animator>();

        // Spawn a new waypoint at the beginning of the game
        FindNewWaypoint();
        StartCoroutine("RandomFlapping");
        SwitchAnimation("Glide");
        
        birdAudio.PlaySound("birdFlying");
        birdAudio.PlaySound("forest");

        StartCoroutine("RandomSounds");
        birdState.SwitchState(BirdStateChanger.BirdState.GoToLanding);

    }

    IEnumerator RandomSounds()
    {
        yield return new WaitForSeconds(Random.Range(8, 20));   
        birdAudio.PlaySound("birdFlying");
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
        if (birdState.currentState == BirdStateChanger.BirdState.Landing && !triggerName.Equals("TakeOff")) return;
        
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

        // For testing spawn a cube at the new waypoint
        if( currentCube !=null) Destroy(currentCube);

        if (testcube != null)
        {
            currentCube = Instantiate(testcube, currentWaypoint, Quaternion.identity);

        currentCube.transform.localScale *= .5f;        }
    }

    public IEnumerator FacePlayer()
    {
        rotationGoal = Quaternion.LookRotation(target.position - transform.position);
        float timePassed = 0;


        while (timePassed < 1)
        {
            Vector3 tempRotation = transform.eulerAngles;
            tempRotation.y = Mathf.Lerp(tempRotation.y, rotationGoal.eulerAngles.y, timePassed);
            tempRotation.z = 0;

            transform.eulerAngles = tempRotation;
            timePassed += Time.deltaTime * .3f;
            yield return new WaitForFixedUpdate();
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("floor"))
        {
            transform.Rotate(Vector3.right * -.3f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (birdState.currentState)
        {
            case BirdStateChanger.BirdState.Hunting:
                BasicFlying();
                break;
            
            case BirdStateChanger.BirdState.GoToLanding:
                BasicFlying();
                break;

            case BirdStateChanger.BirdState.Landing:
                if (grabbedFish)
                {
                    grabbedFish = false;
                    Destroy(prey.gameObject);
                }

                if (!introSequenceDone)
                {
                    print("INTRO SEQUENCE STARTING");
                    introSequenceDone = true;
                    StartCoroutine(nameof(IntroSequence));
                }

                if (landingSpot == handLandingSpot)
                {
                    birdState.customControlsUnlocked = true;
                    foreach (var g in customControlsUI)
                    {
                        g.SetActive(true);
                    }
                }
                transform.position = Vector3.MoveTowards(transform.position, landingSpot.position + Vector3.up * .1f, .4f * Time.deltaTime);
               // FaceTowardMovement();
                ResetXAngle();
                break;
            
            case BirdStateChanger.BirdState.Welcoming:
                BasicFlying();
                break;
            
            case BirdStateChanger.BirdState.Diving:
                currentWaypoint = prey.position;
                //currentWaypoint.y -= Vector3.Distance(prey.position, transform.position);
                BasicFlying();
                turnSpeed *= 1.01f;
                turnSpeed += Mathf.Lerp(.1f, 0, Vector3.Distance(transform.position, prey.position)*.5f);

                if (speed < maxSpeed)
                {
                    speed *= 1.01f;
                }
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

    IEnumerator IntroSequence()
    {
        yield return new WaitForSeconds(8f);
        birdAudio.PlaySound("birdScream");
        yield return new WaitForSeconds(3f);
        print("GOING TO TakeOff");

        birdState.SwitchState(BirdStateChanger.BirdState.TakeOff);
        yield return new WaitForSeconds(15f);
        print("GOING TO Welcoming");

        birdState.SwitchState(BirdStateChanger.BirdState.Welcoming);

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
                birdAudio.PlaySound("birdScream");
                fishBucket.SetActive(true);
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
        var position = transform.position;
        Vector3 dir = position - previousPos;
        rotationGoal = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotationGoal, turnSpeed);

        //transform.rotation = Quaternion.LookRotation(dir.normalized);
        previousPos = position;
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
            
            if (transform.position.y < -12f)
            {
                transform.Rotate(Vector3.right * -.3f);
            }

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

        return angle;

    }
    private void ForwardMovement()
    {
        Transform t = transform;

        t.Translate(Vector3.forward * speed * Time.deltaTime);
        
        direction = (currentWaypoint - t.position).normalized;
        Vector3 eulerRotationGoal = Quaternion.LookRotation(direction).eulerAngles;

        Quaternion flatVersion = Quaternion.Euler(eulerRotationGoal.x,
                                                  eulerRotationGoal.y,
                                                  t.eulerAngles.z);

        t.rotation = Quaternion.Slerp(
            t.rotation,
            flatVersion,
            turnSpeed);

        Vector3 tilt = t.eulerAngles;
        tilt.z = TiltWithReturn();
        t.rotation = Quaternion.Euler(tilt);

        if (transform.position.y < -15f)
        {
            transform.Rotate(Vector3.right * -.3f);
        }

        FlapWingCheck();
    }

    private void FlapWingCheck()
    {
        if (transform.forward.y < -.05f)
        {
            flappngRate = originalFlappngRate * .1f;
            glidingRate = originalGlidingRate * 2;

            SwitchAnimation("Glide");
            anim.ResetTrigger(Flap);
        }
        else if (transform.forward.y > .1f)
        {
            flappngRate = originalFlappngRate * 2;
            glidingRate = originalGlidingRate * .1f;

            SwitchAnimation("Flap");
            anim.ResetTrigger(Glide);

        }
        else
        {
            flappngRate = originalFlappngRate;
            glidingRate = originalGlidingRate;
        }
    }

    public void SwitchAnimationState(BirdStateChanger.BirdState newState)
    {
        print("Switching animation state " + newState);
        switch (newState)
        {
            case BirdStateChanger.BirdState.TakeOff:
                anim.SetBool(OnGround,false);
                SwitchAnimation("Flap");
                SwitchAnimation("TakeOff");

                break;
            case BirdStateChanger.BirdState.Landing:
                
                anim.SetBool(OnGround,true);
                SwitchAnimation("Hover");

                break;
            case BirdStateChanger.BirdState.Diving:
                SwitchAnimation("Diving");
                break;
            case BirdStateChanger.BirdState.Eating:
                anim.SetBool(Eating,true);
                break;
            
        }

    }
}

