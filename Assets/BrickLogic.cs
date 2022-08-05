using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickLogic : MonoBehaviour
{

    private bool _activated;

    private MeshRenderer renderer;
    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<MeshRenderer>();
        renderer.enabled = false;

        Invoke(nameof(ActivateBrick),7.7f);
    }

    void ActivateBrick()
    {
        _activated = true;
    }
    private void OnCollisionEnter(Collision other)
    {
        if (_activated)
        {
            renderer.enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
