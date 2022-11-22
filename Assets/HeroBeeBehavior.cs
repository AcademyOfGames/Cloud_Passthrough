using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Random = UnityEngine.Random;

public class HeroBeeBehavior : MonoBehaviour
{
    //State
    public BeeState currentState;
    public enum BeeState { MeetingPlayer, WatchingPlayer, GoToHand, LandedOnHand
    }

    //Positions
    public float proximityDistance;
    
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
    
    // Start is called before the first frame update
    void Start()
    {
        originalSpeed = speed;
        SwitchStates(BeeState.MeetingPlayer);
        OVRGrabber[] hands = FindObjectsOfType<OVRGrabber>();
       // rightHandLandingSpot = hands[0].transform;
        //leftHandLandingSpot = hands[1].transform;
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
                swingAmount.y = Mathf.Sin(Time.time * swingFrequency) * swingStrength;
                transform.Rotate(swingAmount);
                transform.position = Vector3.MoveTowards(transform.position, wayPoint,speed );
                waypointViz.transform.position = wayPoint;
                DistanceCheck(wayPoint);
                
                

                break;        
            case BeeState.GoToHand:
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

                    if (Vector3.Distance(rightHandLandingSpot.position, t.position) < .1f)
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
                wayPointOrigin = player.position + player.forward *.1f;

                speed = 0f;
                proximityDistance = .02f;
                StartCoroutine(nameof(MovingWatchPoint));
                break;
            case BeeState.GoToHand:
                wayPoint = rightHandLandingSpot.position;
                waypointViz.transform.position = wayPoint;

                proximityDistance = .1f;
                speed = originalSpeed*.1f;
                break;
            case BeeState.LandedOnHand:
                print("Stopping bee audio " + beeAudio.gameObject.name);
                beeAudio.Stop();
                anim.SetBool("Eating",true);

                transform.SetParent(rightHandLandingSpot);
                transform.localPosition = Vector3.zero;
                
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
            float waitTime = Random.Range(1.7f, 5f);
            yield return new WaitForSeconds(waitTime);
            FindNewWayPoint(.6f);
        }
    }

    private void FindNewWayPoint(float waypointSphereScale)
    {
        //magic number for now
        speed = originalSpeed*.6f;
        
        //if player turns bee will always be in front of players face

        
        //find new spot within a certain range of the original point
        wayPoint = Random.insideUnitSphere * .5f + wayPointOrigin;
        wayPoint.x *= waypointSphereScale;
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
