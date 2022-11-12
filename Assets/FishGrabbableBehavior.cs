using UnityEngine;
using UnityEngine.Events;
 
public class FishGrabbableBehavior : OVRGrabbable
{
    [HideInInspector] public UnityEvent OnGrabBegin;
    [HideInInspector] public UnityEvent OnGrabEnd;
 
    public override void GrabBegin(OVRGrabber hand, Collider grabPoint)
    {
        OnGrabBegin.Invoke();
        base.GrabBegin(hand, grabPoint);
    }
 
    public override void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity)
    {
        base.GrabEnd(linearVelocity, angularVelocity);
        OnGrabEnd.Invoke();
    }
}