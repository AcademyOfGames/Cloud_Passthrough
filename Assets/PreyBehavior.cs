using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PreyBehavior : MonoBehaviour
{
    private Vector3 ogPos;
    private void OnCollisionEnter(Collision other)
    {
        other.collider.GetComponent<BirdStateChanger>().SwitchState(BirdStateChanger.BirdState.Hunting);
        transform.SetParent(other.transform);
    }

    // Start is called before the first frame update
    void Start()
    {
        ogPos = transform.position;
        GetComponent<Rigidbody>().AddForce(Abs(Random.insideUnitSphere) *500f);   
    }

    Vector3 Abs(Vector3 v)
    {
        Vector3 absVector = new Vector3(v.x, Mathf.Abs(v.y), v.z);
        return absVector;
    }
    // Update is called once per frame
    void Update()
    {
                GetComponent<Rigidbody>().AddForce(Vector3.up *1.5f);   

    }
}
