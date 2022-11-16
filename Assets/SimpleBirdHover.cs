using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBirdHover : MonoBehaviour
{
    public Transform target;
    private Transform t;
    public float speed;

    private Vector3 center;

    private Vector3 lastPos;
    // Start is called before the first frame update
    void Start()
    {
        t = transform;
        speed += Random.Range(-speed * .1f, speed * .1f);
        var position = target.position;
        center = new Vector3(position.x,transform.position.y, position.y);
        center.x += Random.Range(-3, 3);
        center.z += Random.Range(-3, 3);
        lastPos = t.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.RotateAround(center, Vector3.up,speed);
        var position = t.position;
        transform.rotation = Quaternion.LookRotation(lastPos - position);
        lastPos = position;
    }
}
