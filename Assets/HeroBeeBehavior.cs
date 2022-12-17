using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Random = UnityEngine.Random;

public class HeroBeeBehavior : MonoBehaviour
{
    //State
    public BeeState currentState;
    public enum BeeState { MeetingPlayer, WatchingPlayer, GoToHand, LandedOnHand, HandControls,
        Explore
    }
    private BeeStateChanger beeControls;
    //Positions
    public float proximityDistance;
    public Transform flower;
    public Transform player;
    public Transform rightHandLandingSpot; 
    
    [SerializeField] private Vector3 wayPointOrigin;
    [SerializeField] private float speed;
    [SerializeField] private Vector3 wayPoint;

    private float originalSpeed;

    public GameObject waypointViz;

    public Animator anim;
    public AudioSource beeAudio;

    private Vector3 swingAmount = Vector3.zero;
    public float swingStrength;
    public float swingFrequency;

    //temporarily public so I can see it without seeing all the debug private vars
    public float distance;
    Vector3 lastPos;

    public float controllerMovementSpeed;
    public float controllerRotationSpeed;
    public float controllerVerticalSpeed;

    private Vector2 tiltDir;

    public float tiltIntensity = 10;

    public Transform beeMesh;

    public float maxSpeed = 3.5f;
    public float acceleration = 1.15f;
    // Start is called before the first frame update
    void Start()
    {
        beeControls = GetComponent<BeeStateChanger>();

        lastPos = transform.position;
        originalSpeed = speed;
        
        
        SwitchStates(BeeState.MeetingPlayer);
        
        
        OVRGrabber[] hands = FindObjectsOfType<OVRGrabber>();
       // rightHandLandingSpot = hands[0].transform;
        //leftHandLandingSpot = hands[1].transform;
        transform.parent = null;
        transform.localScale = new Vector3(0.04f, 0.04f, 0.04f);
        //bee starts going to players face
    }

    private float moveSpeedAcceleration;
    void FixedUpdate()
    {
        switch (currentState)
        {
            case BeeState.Explore:
                transform.LookAt(wayPoint);
                transform.Translate(Vector3.forward * speed);
                DistanceCheck(wayPoint);
                break;
            
            case BeeState.HandControls:
                Vector3 moveDir = new Vector3(beeControls.rMovement.x * controllerMovementSpeed,
                    beeControls.lMovement.y * controllerVerticalSpeed,
                    beeControls.rMovement.y * controllerMovementSpeed) * Time.deltaTime;
                

                if (moveDir.magnitude > .004f  )
                {
                    if (moveSpeedAcceleration < maxSpeed)
                    {
                        moveSpeedAcceleration += Time.deltaTime * acceleration;
                    }
                }
                else
                {
                    moveSpeedAcceleration = 0;
                }
                
                transform.Translate(moveDir*moveSpeedAcceleration) ;
                print(beeControls.lMovement);
                transform.Rotate(Vector3.up * beeControls.lMovement.x* controllerRotationSpeed * Time.deltaTime);
                break;
            
            case BeeState.MeetingPlayer:
                transform.LookAt(wayPoint);
                
                //translate already takes account transform so it should just be vector3
                transform.Translate(Vector3.forward * speed );
                wayPoint = player.position + new Vector3(player.forward.x, 0, player.forward.z) *.3f;
                
                if(waypointViz!=null){
                    waypointViz.transform.position = wayPoint;
                }
                DistanceCheck(player.position);

                break;
            case BeeState.WatchingPlayer:
                transform.LookAt((player.position + wayPoint )/2);

                //swingAmount.y = Mathf.Sin(Time.time * swingFrequency) * swingStrength;

                transform.position = Vector3.MoveTowards(transform.position, wayPoint,speed );
                if(waypointViz != null)
                {
                    waypointViz.transform.position = wayPoint;
                }
                swingAmount.y = Mathf.Sin(Time.time * swingFrequency) * swingStrength;
                transform.Rotate(swingAmount);

                DistanceCheck(wayPoint);
                
                

                break;        
            case BeeState.GoToHand:
                //transform.LookAt(wayPoint);
                transform.position = Vector3.MoveTowards(transform.position, rightHandLandingSpot.position,speed );
                DistanceCheck(rightHandLandingSpot.position);

                break;
        }

        CalculateTilt();
    }

    private void CalculateTilt()
    {
        
        Vector3 targetAngle = Vector3.zero;

        Vector3 moveSpeed = lastPos - transform.position;
        var localVelocity = transform.InverseTransformDirection(moveSpeed)*110f;
        if (localVelocity.magnitude > .2)
        {
            float forwardTilt = localVelocity.z;
            forwardTilt = (forwardTilt + 1) *.5f;
            if (forwardTilt > 0)
            {
                targetAngle = beeMesh.localEulerAngles;
                targetAngle.x = Mathf.Lerp(20, -20, forwardTilt);
                beeMesh.localEulerAngles = targetAngle;
                

            }

            float sideTilt = localVelocity.x;
            sideTilt = (sideTilt + 1) * .5f;
            if (sideTilt > 0)
            {
                
                
                targetAngle = beeMesh.localEulerAngles;
                targetAngle.z = Mathf.Lerp(-25, 25, sideTilt);
                beeMesh.localEulerAngles = targetAngle;
            }
        }
        else
        {

            beeMesh.localRotation = Quaternion.Slerp(beeMesh.localRotation, Quaternion.identity, .1f);

        }

        lastPos = transform.position;
    }


    /// <summary>
    /// check how far bee is from objective
    /// </summary>
    /// <param name="obj">objective</param>
    private void DistanceCheck(Vector3 obj)
    {
        Transform t = transform;
         distance = Vector3.Distance(t.position, obj);
        
        if (distance <= proximityDistance)
        {
            switch (currentState)
            {
                case BeeState.Explore:
                    FindNewWayPoint();
                    break;
                // bee goes to player and starts to hover in front of them
                case BeeState.MeetingPlayer:
                    SwitchStates(BeeState.WatchingPlayer);

                    break;
                case BeeState.WatchingPlayer:
                    speed = 0f;

                    if (Vector3.Distance(rightHandLandingSpot.position, t.position) < .3f)
                    {

                        SwitchStates(BeeState.GoToHand);
                    }
                    
                    break;
                
                case BeeState.GoToHand:
                    
                    SwitchStates(BeeState.LandedOnHand);

                    break;
            }
        }
    }
    
    IEnumerator TakeOff()
    {
        float timePassed = 0;
        Vector3 adjustedPos = transform.position;
        while (timePassed < 1.5)
        {
            adjustedPos.y += .004f;
            transform.position = adjustedPos;
            timePassed += Time.deltaTime;
            yield return null;
        }
    }

    public void SwitchStates(BeeState newState)
    {
        if (currentState == newState) return;
        currentState = newState;
        
        
        GetComponent<Animator>().enabled = true;

        switch (newState)
        {
            case BeeState.Explore:
                anim.SetBool("Walking", false);
                anim.SetBool("Eating", false);
                transform.parent = null;
                speed = originalSpeed * .4f;
                wayPointOrigin = flower.position + Vector3.up * 2.5f;
                FindNewWayPoint();
                break;
            case BeeState.HandControls:
                speed = originalSpeed;
                anim.SetBool("Walking", false);
                anim.SetBool("Eating", false);
                transform.SetParent(null);
                transform.rotation = Quaternion.identity;
                StartCoroutine(nameof(TakeOff));
                break;
            case BeeState.MeetingPlayer:
       
                break;
            //In watch player, bee will look at the player and sporadically switch spots
            case BeeState.WatchingPlayer:
                wayPointOrigin = player.position + new Vector3( player.forward.x,0, player.forward.z) *.2f;

                speed = 0f;
                proximityDistance = .02f;
                StartCoroutine(nameof(MovingWatchPoint));
                break;
            case BeeState.GoToHand:
                wayPoint = rightHandLandingSpot.position;
                if(waypointViz != null)
                {
                    waypointViz.transform.position = wayPoint;

                }

                proximityDistance = .01f;
                speed = originalSpeed*.1f;
                break;
            case BeeState.LandedOnHand:
                foreach (var flower in FindObjectsOfType<FlowerBehavior>())
                {
                    flower.ActivateSecondFlowers();
                }
                beeControls.UnlockControls(true);
                beeAudio.Stop();
                anim.SetBool("Eating",true);

                Transform t = transform;
                t.SetParent(rightHandLandingSpot);
                t.localPosition = Vector3.zero;
                t.localEulerAngles = new Vector3(0,0,-90);
                GetComponent<Animator>().enabled = false;
                
                speed = 0;
                break;
        }
        
    }

    /// <summary>
    /// randomly find new waypoint
    /// </summary>
    /// <returns></returns>
    IEnumerator MovingWatchPoint()
    {
        while (currentState == BeeState.WatchingPlayer)
        {
            float waitTime = Random.Range(2.4f, 5.2f);
            yield return new WaitForSeconds(waitTime);
            FindNewWayPoint(.5f);
        }
    }

    private void FindNewWayPoint(float waypointSphereScale)
    {
        //magic number for now
        speed = originalSpeed*.4f;
        
        //if player turns bee will always be in front of players face

        
        //find new spot within a certain range of the original point
        wayPoint = Random.insideUnitSphere * .5f + wayPointOrigin;
        wayPoint.x *= waypointSphereScale;
    }
    
    private void FindNewWayPoint()
    {
        //magic number for now
        
        //if player turns bee will always be in front of players face

        //find new spot within a certain range of the original point
        wayPoint = Random.insideUnitSphere + wayPointOrigin;
    }
}
