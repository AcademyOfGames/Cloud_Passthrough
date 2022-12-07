using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedBehavior : MonoBehaviour
{
    private SeedGrabbableBehavior _grabInfo;
    public Transform handPos;
    public GameObject seedParticles;
    public FlowerBehavoir flowers;

    private bool grabbed;
    private Transform snapPos;

    private Vector3 handSpeed;
    private Vector3 lastPosition;
    public GameObject controllerVisual;
    ParticleSystem seedParticleSystem;

    public GameObject seedVisual;

    void Awake()
    {
        _grabInfo = GetComponent<SeedGrabbableBehavior>();
        seedParticleSystem = seedParticles.GetComponent<ParticleSystem>();
    }
    private void OnEnable() {
        _grabInfo.OnGrabBegin.AddListener(IsGrabbed);
        _grabInfo.OnGrabEnd.AddListener(IsReleased);
    }
 
    private void OnDisable() {
        _grabInfo.OnGrabBegin.RemoveListener(IsGrabbed);
        _grabInfo.OnGrabEnd.RemoveListener(IsReleased);
    }

    void IsGrabbed()
    {
        controllerVisual.SetActive(false);
        seedVisual.SetActive(true);
        grabbed = true;
        snapPos = _grabInfo.grabbedBy.transform;
    }

    void IsReleased()
    {
        flowers.gameObject.SetActive(true);
        Invoke("ControlVisualOn", 2);
        //seedParticleSystem.startSpeed = handSpeed.magnitude *5;
        print("handSpeed.magnitude " + handSpeed.magnitude);
        grabbed = false;

        seedParticles.SetActive(true);
        seedVisual.SetActive(false);
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().AddForce(handSpeed * 1800);
    }

    public void ControlVisualOn()
    {
        controllerVisual.SetActive(true);

    }
    private void FixedUpdate()
    {
        handSpeed =  transform.position -lastPosition;
        lastPosition = transform.position;
        
        if (grabbed)
        {
            transform.position = snapPos.position;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("floor"))
        {
            Vector3 growPoint = other.GetContact(0).point;
            growPoint.y -= 2;
            IEnumerator grow =  flowers.Grow(growPoint);
            StartCoroutine(grow);
        }
    }
}
