using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformUtils : MonoBehaviour
{
    public bool inFrontOfPlayer;
    public bool lookAtPlayer;
    public Vector3 offset;
    public Transform player;
    // Start is called before the first frame update
    void Start()
    {
        print("Distance x " + name + " =  " + (transform.position.x - player.position.x));
        print("Distance y " + name + " =  " + (transform.position.y - player.position.y));
        print("Distance z " + name + " =  " + (transform.position.z - player.position.z));
        
        
        if(inFrontOfPlayer)
        {
            transform.position = player.position;
            transform.Translate(offset);
        }

        if (lookAtPlayer)
        {
            transform.LookAt(player.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
