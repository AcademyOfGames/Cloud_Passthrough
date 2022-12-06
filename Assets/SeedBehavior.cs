using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedBehavior : MonoBehaviour
{
    private SeedGrabbableBehavior _grabInfo;
    public Transform handPos;
    public GameObject seedParticles;

    public GameObject flowers;

    private bool grabbed;
    private Transform snapPos;

    private Vector3 handSpeed;
    private Vector3 lastPosition;
    ParticleSystem seedParticleSystem;
    

    void Awake()
    {
        _grabInfo = GetComponent<SeedGrabbableBehavior>();
        print("***Grabbing particle system from " + seedParticles.name);
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
        GetComponent<MeshRenderer>().enabled = true;
        grabbed = true;
        snapPos = _grabInfo.grabbedBy.transform;
    }

    void IsReleased()
    {
        seedParticles.SetActive(true);

        StartCoroutine(ScaleDown());
        flowers.SetActive(true);

        seedParticleSystem.startSpeed = handSpeed.magnitude *5;
        seedParticles.transform.parent = null; 
        //Destroy(gameObject, seedParticles.GetComponent<ParticleSystem>().main.startLifetime.constant);
    }

    private void FixedUpdate()
    {
        handSpeed = lastPosition - transform.position;
        lastPosition = transform.position;

        if (grabbed)
        {
            transform.position = snapPos.position;
            transform.rotation = Quaternion.LookRotation(snapPos.forward);
        }
    }

    IEnumerator ScaleDown( )
    {
        Transform t = transform;
        GetComponent<SphereCollider>().enabled = false;

        while (t.localScale.x <=.01f)
        {
            t.localScale *= .6f;

            yield return new WaitForFixedUpdate();
        }

        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().useGravity = true;

    }
}
