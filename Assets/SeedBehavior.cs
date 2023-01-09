using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedBehavior : MonoBehaviour
{
    private SeedGrabbableBehavior _grabInfo;
    public Transform handPos;
    public GameObject seedParticles;
    public FlowerBehavior flowers;

    private bool grabbed;
    private Transform snapPos;

    private Vector3 handSpeed;
    private Vector3 lastPosition;
    public GameObject controllerVisual;
    ParticleSystem seedParticleSystem;

    public GameObject seedVisual;

    private ControlUIManager controlUIManager;

    void Awake()
    {
        controlUIManager = FindObjectOfType<ControlUIManager>();
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
        controlUIManager.ChangeSeedText("Throw Seeds");
        snapPos = _grabInfo.grabbedBy.transform;
    }

    void IsReleased()
    {
        controlUIManager.ChangeSeedText("Grab Seeds");

        flowers.gameObject.SetActive(true);
        Invoke("ControlVisualOn", 2);
        //seedParticleSystem.startSpeed = handSpeed.magnitude *5;
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("floor"))
        {
            IEnumerator grow =  flowers.Grow(other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position));
            StartCoroutine(grow);
        }
    }
}
