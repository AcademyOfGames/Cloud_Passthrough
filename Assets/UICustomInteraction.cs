using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRKeys;

public class UICustomInteraction : MonoBehaviour
{
    private LineRenderer line;
    
    // Start is called before the first frame update
    void Awake()
    {
        line = GetComponent<LineRenderer>();
    }

    private void FixedUpdate()
    {
        line.SetPosition(0, transform.position);
        line.SetPosition(1, transform.position + transform.forward * 10);
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
        {
            ShootRay();
        }
    }
    public void ToggleLaser(bool on)
    {
        line.enabled = on;
    }

     void ShootRay()
     {
         RaycastHit hit;
         Ray ray = new Ray(transform.position, transform.forward);
         if (Physics.Raycast(ray, out hit))
         {
             GenericVRClick click = hit.collider.GetComponent<GenericVRClick>();
             if (click != null)
             {
                 click.Click();
             }
             
             Key key = hit.collider.GetComponent<Key>();
             if (key != null)
             {
                 key.HandleTriggerEnter();
             }
             
             OnClickVRCollider colliderClick = hit.collider.GetComponent<OnClickVRCollider>();
             
             if (colliderClick != null)
             {
                 colliderClick.Click();
             }
         }
    }
}
