using UnityEngine;
using UnityEngine.Events;
 
public class OVRGrabbableExtended : OVRGrabbable
{
    [HideInInspector] public UnityEvent OnGrabBegin;
    [HideInInspector] public UnityEvent OnGrabEnd;
 
    public override void GrabBegin(OVRGrabber hand, Collider grabPoint)
    {
        print("Grabbing begin");
        OnGrabBegin.Invoke();
        base.GrabBegin(hand, grabPoint);
    }
 
    public override void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity)
    {
        print("Grabbing end");

        base.GrabEnd(linearVelocity, angularVelocity);
        OnGrabEnd.Invoke();
    }
}