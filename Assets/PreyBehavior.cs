using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using TMPro;

public class PreyBehavior : MonoBehaviour
{


    public TextMeshProUGUI debugTemp;

    private Vector3 _ogPos;
    public Transform birdHand;
    private OVRGrabbableExtended _grabInfo;
    private BirdStateChanger _birdState;

    private bool _isGrabbed;
    private void OnTriggerEnter(Collider other)
    {

        var bird = other.GetComponent<BirdMovement>();
        if (bird != null)
        {
            if (bird.grabbedFish) return;
            print("fish caught "+ bird.fishCaught);
            bird.prey = transform;

            bird.fishCaught++;
            bird.grabbedFish = true;
            _birdState.GetComponent<BirdMovement>().landingSpot = bird.branchLandingSpot;
            _birdState.SwitchState(BirdStateChanger.BirdState.GoToLanding);            
            GetComponent<Rigidbody>().useGravity = false;
            transform.SetParent(birdHand);
            StartCoroutine(nameof(LerpToHand));    
        }
    }
 
    private void OnEnable() {
        // listen for grabs
        _grabInfo.OnGrabBegin.AddListener(IsGrabbed);
        _grabInfo.OnGrabEnd.AddListener(IsReleased);
    }
 
    private void OnDisable() {
        // stop listening for grabs
        _grabInfo.OnGrabBegin.RemoveListener(IsGrabbed);
        _grabInfo.OnGrabEnd.RemoveListener(IsReleased);
    }

    IEnumerator LerpToHand()
    {
        while (transform.localPosition.magnitude > .01f)
        {
            transform.localPosition *= .9f;
            yield return null;
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        _birdState = FindObjectOfType<BirdStateChanger>();
        _grabInfo = GetComponent<OVRGrabbableExtended>();
        _ogPos = transform.position;
        //GetComponent<Rigidbody>().AddForce(Abs(Random.insideUnitSphere) *500f);   
    }

    Vector3 Abs(Vector3 v)
    {
        Vector3 absVector = new Vector3(v.x, Mathf.Abs(v.y), v.z);
        return absVector;
    }

    void IsGrabbed()
    {
        _birdState.SwitchState(BirdStateChanger.BirdState.Welcoming);
        _birdState.GetComponent<BirdMovement>().anim.SetBool("Eating", false);

    }
    
    void IsReleased()
    {
        if ( _birdState.GetComponent<BirdMovement>().grabbedFish) return;

        _birdState.GetComponent<BirdMovement>().prey = transform;
        //print("FishReleased ");
        _birdState.SwitchState(BirdStateChanger.BirdState.Diving);

    }
    // Update is called once per frame
    void Update()
    {

    }
}
