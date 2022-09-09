using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class DeerMovement : MonoBehaviour
{
    public float speed;

    public Transform waypoint;
    
    public enum AnimalStates {Walking, Eating, StopAndLook,
        Trotting
    }

    public AnimalStates currentState = AnimalStates.Walking;

    private Animator anim;
    
    public float waypointProximity = 3f;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
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
            case AnimalStates.StopAndLook:
                anim.SetTrigger("StopAndLook");
                StartCoroutine(WaitAndSwitchState(AnimalStates.Eating, 2f));
                break;
            case AnimalStates.Eating:
                anim.SetBool("Eating",true);
                StartCoroutine(WaitAndSwitchState(AnimalStates.Trotting, 3f));
                break;
            case AnimalStates.Trotting:
                anim.SetBool("Eating",false);
                anim.SetBool("Walking",false);

                anim.SetBool("Trotting",true);
                StartCoroutine(WaitAndSwitchState(AnimalStates.Trotting, 5f));
                break;
        }    
    }

    IEnumerator WaitAndSwitchState(AnimalStates nextState, float time)
    {
        yield return new WaitForSeconds(time);
        SwitchState(nextState);
    }
    
    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case AnimalStates.Walking:
                transform.LookAt(waypoint.position);
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
        float waypointDistance = Vector3.Distance(waypoint.position, transform.position);
        
        if(waypointDistance < waypointProximity)
        {
            if (currentState == AnimalStates.Walking)
            {
                SwitchState(AnimalStates.StopAndLook);
            }
        }
    }
}
