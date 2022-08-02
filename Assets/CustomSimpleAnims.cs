using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomSimpleAnims : MonoBehaviour
{
    public bool deactivateAfterTime;

    public float time;
    // Start is called before the first frame update
    void Start()
    {
        if (deactivateAfterTime)
        {
            Destroy(gameObject, time);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
