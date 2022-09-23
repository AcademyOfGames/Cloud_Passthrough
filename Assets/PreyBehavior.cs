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
    public float throwMultiplier;
    private OVRGrabbableExtended _grabInfo;
    private BirdStateChanger _birdState;

    private bool _isGrabbed;
    private Vector3 _velocity;
    private Vector3 _lastPos;

    private Rigidbody _rb;

    int _frameCount;
    private bool _fishThrown;
    private StumpBehavior _stump;

    private void OnTriggerEnter(Collider other)
    {

        var bird = other.GetComponent<BirdMovement>();
        if (bird != null)
        {
            FindObjectOfType<GoogleSheets>().AddEventData("Grabbed a fish.", SystemInfo.deviceUniqueIdentifier);

            if (bird.grabbedFish) return;
            _fishThrown = true;
            bird.prey = transform;
            _isGrabbed = true;
            bird.fishCaught++;
            bird.grabbedFish = true;

            _birdState.GetComponent<BirdMovement>().landingSpot = bird.branchLandingSpot;
            _birdState.SwitchState(BirdStateChanger.BirdState.GoToLanding);            
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<Rigidbody>().isKinematic = true;
            transform.SetParent(birdHand);
            StartCoroutine(nameof(LerpToHand));    
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("floor") && !_fishThrown)
        {
            transform.position = _stump.preySpawner.position;
            transform.eulerAngles = new Vector3(286.403046f, 283.090424f, 347.42511f);
            _rb.velocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            _rb.mass = 1000;
            StartCoroutine(nameof(ResetFishMass));
        }
    }
    IEnumerator ResetFishMass()
    {
        yield return new WaitForSeconds(2f);
        _rb.mass = 1;

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
            transform.localPosition *= .1f;
            yield return null;
        }
        Vector3 newRotation = transform.eulerAngles;
        newRotation.y = 0;
        transform.eulerAngles = newRotation;
    }

    // Start is called before the first frame update
    void Awake()
    {
        _stump = FindObjectOfType<StumpBehavior>();
        _birdState = FindObjectOfType<BirdStateChanger>();
        _grabInfo = GetComponent<OVRGrabbableExtended>();
        _ogPos = transform.position;
        _rb= GetComponent<Rigidbody>();
    }
    
    void IsGrabbed()
    {
        _birdState.SwitchState(BirdStateChanger.BirdState.Welcoming);
        _birdState.GetComponent<BirdMovement>().anim.SetBool("Eating", false);

    }
    
    void IsReleased()
    {
        if ( _birdState.GetComponent<BirdMovement>().grabbedFish) return;

        _fishThrown = true;
        GetComponent<Rigidbody>().AddForce(_velocity * throwMultiplier);
        _birdState.GetComponent<BirdMovement>().prey = transform;
        _birdState.SwitchState(BirdStateChanger.BirdState.Diving);
    }
    
    // Update is called once per frame
    void Update()
    {
        _frameCount++;
        _velocity = transform.position - _lastPos;

        if(_frameCount % 3 == 0)
        {
            _frameCount = 1;
            _lastPos = transform.position;

        }
        if (_isGrabbed)
        {
            _rb.isKinematic = true;
        }
    }
}
