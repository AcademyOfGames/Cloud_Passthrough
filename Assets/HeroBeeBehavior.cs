using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Random = UnityEngine.Random;

public class HeroBeeBehavior : MonoBehaviour
{
    //State
    public BeeState currentState;
    public enum BeeState { MeetingPlayer, WatchingPlayer,HandWandering,
        GoToFlower,
        LandedOnFlower
    }


    
    //Positions
    public float proximityDistance;
    
    public Transform player;
    public Transform rightHandLandingSpot;
    public Transform leftHandLandingSpot;
    private Transform handHoldingBee;
    
    [SerializeField] private Vector3 wayPointOrigin;
    [SerializeField] private float speed;
    [SerializeField] private Vector3 wayPoint;

    private float originalSpeed;

    public GameObject waypointViz;

    public Animator anim;
    public AudioSource beeAudio;

    //temporarily public so I can see it without seeing all the debug private vars
    public float distance;
    
    // Start is called before the first frame update
    void Start()
    {
        originalSpeed = speed;
        SwitchStates(BeeState.MeetingPlayer);
        OVRGrabber[] hands = FindObjectsOfType<OVRGrabber>();
        rightHandLandingSpot = hands[0].transform;
        leftHandLandingSpot = hands[1].transform;
        transform.parent = null;
        transform.localScale = new Vector3(0.04f, 0.04f, 0.04f);
        //bee starts going to players face
    } 

    void FixedUpdate()
    {

        switch (currentState)
        {
            case BeeState.MeetingPlayer:
                transform.LookAt(wayPoint);
                
                //translate already takes account transform so it should just be vector3
                transform.Translate(Vector3.forward * speed );
                wayPoint = player.position + player.forward;
                waypointViz.transform.position = wayPoint;

                DistanceCheck(player.position);

                break;
            case BeeState.WatchingPlayer:
                transform.LookAt(player);
                transform.position = Vector3.MoveTowards(transform.position, wayPoint,speed );
                waypointViz.transform.position = wayPoint;

                DistanceCheck(wayPoint);

                break;        
            case BeeState.GoToFlower:
                transform.LookAt(wayPoint);
                transform.position = Vector3.MoveTowards(transform.position, wayPoint,speed );
                DistanceCheck(wayPoint);

                break;
            
            
        }
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
                // bee goes to player and starts to hover in front of them
                case BeeState.MeetingPlayer:
                    SwitchStates(BeeState.WatchingPlayer);

                    break;
                case BeeState.WatchingPlayer:
                    speed = 0f;

                    if (Vector3.Distance(rightHandLandingSpot.position, t.position) < .5f)
                    {
                        handHoldingBee = rightHandLandingSpot;

                        SwitchStates(BeeState.GoToFlower);
                    }
                    
                    if(Vector3.Distance(leftHandLandingSpot.position, t.position) < .5f)
                    {
                        handHoldingBee = leftHandLandingSpot;
                        SwitchStates(BeeState.GoToFlower);
                    }
                    break;
                
                case BeeState.GoToFlower:
                    SwitchStates(BeeState.LandedOnFlower);

                    break;
            }
        }
    }

    private void SwitchStates(BeeState newState)
    {
        if (currentState == newState) return;
        currentState = newState;

        switch (newState)
        {
            case BeeState.MeetingPlayer:
       
                break;
            //In watch player, bee will look at the player and sporadically switch spots
            case BeeState.WatchingPlayer:
                wayPointOrigin = player.position + player.forward;

                speed = 0f;
                proximityDistance = .02f;
                StartCoroutine(nameof(MovingWatchPoint));
                break;
            case BeeState.GoToFlower:
                wayPoint = handHoldingBee.position;
                proximityDistance = .1f;
                speed = originalSpeed*.1f;
                break;
            case BeeState.LandedOnFlower:
                beeAudio.Stop();
                anim.SetBool("Eating",true);
                transform.SetParent(handHoldingBee);
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
            float waitTime = Random.Range(1f, 5f);
            yield return new WaitForSeconds(waitTime);
            FindNewWayPoint();
        }
    }

    private void FindNewWayPoint()
    {
        //magic number for now
        speed = originalSpeed*.6f;
        
        //if player turns bee will always be in front of players face

        
        //find new spot within a certain range of the original point
        wayPoint = Random.insideUnitSphere * .5f + wayPointOrigin;
    }
}
