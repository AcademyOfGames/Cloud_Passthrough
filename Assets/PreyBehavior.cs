using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using TMPro;

public class PreyBehavior : MonoBehaviour
{


    public TextMeshProUGUI debugTemp;

    private Vector3 ogPos;
    public Transform birdHand;
    private OVRGrabbableExtended grabInfo;
    private BirdStateChanger birdState;

    private bool isGrabbed;
    private void OnTriggerEnter(Collider other)
    {
        var bird = other.GetComponent<BirdStateChanger>();
        if (bird != null)
        {
            birdState.GetComponent<BirdMovement>().landingSpot = birdState.GetComponent<BirdMovement>().branchLandingSpot;
            birdState.SwitchState(BirdStateChanger.BirdState.GoToLanding);            
            GetComponent<Rigidbody>().useGravity = false;
            transform.SetParent(birdHand);
            StartCoroutine("LerpToHand");    
        }
    }
 
    private void OnEnable() {
        // listen for grabs
        grabInfo.OnGrabBegin.AddListener(IsGrabbed);
        grabInfo.OnGrabEnd.AddListener(IsReleased);
    }
 
    private void OnDisable() {
        // stop listening for grabs
        grabInfo.OnGrabBegin.RemoveListener(IsGrabbed);
        grabInfo.OnGrabEnd.RemoveListener(IsReleased);
    }

    IEnumerator LerpToHand()
    {
        while (transform.localPosition.magnitude > .01f)
        {
            print(transform.localPosition);
            transform.localPosition *= .9f;
            yield return null;
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        birdState = FindObjectOfType<BirdStateChanger>();
        grabInfo = GetComponent<OVRGrabbableExtended>();
        ogPos = transform.position;
        //GetComponent<Rigidbody>().AddForce(Abs(Random.insideUnitSphere) *500f);   
    }

    Vector3 Abs(Vector3 v)
    {
        Vector3 absVector = new Vector3(v.x, Mathf.Abs(v.y), v.z);
        return absVector;
    }

    void IsGrabbed()
    {
        isGrabbed = true;
    }
    
    void IsReleased()
    {
        isGrabbed = false;
        birdState.GetComponent<BirdMovement>().prey = transform;
        print("FishReleased ");

        birdState.SwitchState(BirdStateChanger.BirdState.Diving);

    }
    // Update is called once per frame
    void Update()
    {

//        if(transform.parent)
        if (isGrabbed)
        {
            debugTemp.text = "FISH GRABBED";
            print("FishGrabbed");
            //birdState.GetComponent<BirdMovement>().landingSpot = GetComponent<BirdMovement>().branchLandingSpot;
            birdState.SwitchState(BirdStateChanger.BirdState.Welcoming);
        }
        
                //GetComponent<Rigidbody>().AddForce(Vector3.up *1.5f);   

    }
}
