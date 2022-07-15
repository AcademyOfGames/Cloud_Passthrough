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
            bird.SwitchState(BirdStateChanger.BirdState.Hunting);
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
    }
    // Update is called once per frame
    void Update()
    {
        if (isGrabbed)
        {
            debugTemp.text = "FISH GRABBED";
            birdState.GetComponent<BirdMovement>().prey = transform;
            birdState.SwitchState(BirdStateChanger.BirdState.Diving);
        }
                //GetComponent<Rigidbody>().AddForce(Vector3.up *1.5f);   

    }
}
