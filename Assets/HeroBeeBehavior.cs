using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Random = UnityEngine.Random;

public class HeroBeeBehavior : MonoBehaviour
{
    public Transform player;
    public enum BeeState { MeetingPlayer, WatchingPlayer,HandWandering }
    public BeeState currentState = BeeState.MeetingPlayer;

    public float proximityDistance;

    
    [SerializeField] private Vector3 wayPointOrigin;
    [SerializeField] private float speed;
    [SerializeField] private Vector3 wayPoint;

    private float originalSpeed;

    public GameObject waypointViz;

    //temporarily public so I can see it without seeing all the debug private vars
    public float distance;
    // Start is called before the first frame update
    void Start()
    {
        originalSpeed = speed;
        
        //bee starts going to players face
    } 

    void FixedUpdate()
    {

        switch (currentState)
        {
            case BeeState.MeetingPlayer:
                transform.LookAt(player);
                
                //translate already takes account transform so it should just be vector3
                transform.Translate(Vector3.forward * speed );
                wayPoint = player.position + player.forward;
                DistanceCheck(player.position);

                break;
            case BeeState.WatchingPlayer:
                transform.LookAt(player);
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
            //In watch player, bee will look at the player and sporadically switch spots
            case BeeState.WatchingPlayer:
                speed = 0f;
                proximityDistance = .1f;
                StartCoroutine(nameof(MovingWatchPoint));
                break;
        }
        
    }

    /// <summary>
    /// randomly find new waypoint
    /// </summary>
    /// <returns></returns>
    IEnumerator MovingWatchPoint()
    {
        float waitTime = Random.Range(1f, 3f);
        print("finding new point waitTime " + waitTime + " currentState " + currentState);

        while (currentState == BeeState.WatchingPlayer)
        {
            yield return new WaitForSeconds(waitTime);
            waitTime = Random.Range(1f, 3f);
            FindNewWayPoint();
        }
    }

    private void FindNewWayPoint()
    {
        //magic number for now
        speed = originalSpeed*.4f;
        
        //if player turns bee will always be in front of players face
        wayPointOrigin = player.position + player.forward;

        
        //find new spot within a certain range of the original point
        wayPoint = Random.insideUnitSphere + wayPointOrigin;
        if(waypointViz != null) waypointViz.transform.position = wayPoint;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (currentState == BeeState.WatchingPlayer && other.CompareTag("Hand"))
        {
            SwitchStates(BeeState.HandWandering);
        }
    }
}
