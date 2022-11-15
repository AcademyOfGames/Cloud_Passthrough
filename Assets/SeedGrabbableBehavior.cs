using UnityEngine;
using UnityEngine.Events;
 
public class SeedGrabbableBehavior : OVRGrabbable
{
    [HideInInspector] public UnityEvent OnGrabBegin;
    [HideInInspector] public UnityEvent OnGrabEnd;
 
    public override void GrabBegin(OVRGrabber hand, Collider grabPoint)
    {
        base.GrabBegin(hand, grabPoint);

        OnGrabBegin.Invoke();
    }
 
    public override void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity)
    {
        base.GrabEnd(linearVelocity, angularVelocity);
        OnGrabEnd.Invoke();
    }
}