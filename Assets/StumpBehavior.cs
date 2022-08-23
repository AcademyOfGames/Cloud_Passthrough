using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StumpBehavior : MonoBehaviour
{
    public Rigidbody[] fishRbs;

    public GameObject poofEffect;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!other.gameObject.CompareTag("floor")) return;
        foreach (var rb in fishRbs)
        {
            if (rb == null) continue;
            rb.useGravity = true;
            rb.isKinematic = false;
        }
        poofEffect.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
