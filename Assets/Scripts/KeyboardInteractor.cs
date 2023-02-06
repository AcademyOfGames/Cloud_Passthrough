using System;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using VRKeys;

public class KeyboardInteractor : MonoBehaviour
{
    
    public InputActionReference reference = null;

    public delegate void StateChange(XRController controller);
    public event StateChange OnButtonDown;
    public event StateChange OnButtonUp;
    
    public InputHelpers.Button button = InputHelpers.Button.None;

    private bool previousPress;

    void Update()
    {
        HandleState(GetComponent<XRController>());
    }
    public void HandleState(XRController controller)
    {
        if (controller.inputDevice.IsPressed(button, out bool pressed, controller.axisToPressThreshold))
        {
            if (previousPress != pressed)
            {
                print("StateHandled");
                previousPress = pressed;
                if (pressed)
                {
                    ShootRay();
                }
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        reference.action.started += ClickKey;
        ActionBasedController controller = GetComponent<ActionBasedController>();

    }
    
    void OnDestroy()
    {
        reference.action.started -= ClickKey;

    }
    void ClickKey(InputAction.CallbackContext context)
    {
        ShootRay();
        
    }

    public void ShootRay()
    {
    print("RayShot");
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if(Physics.Raycast(ray,out hit))
        {
            print("Hit " + hit.collider.name);
            LetterKey key = hit.collider.GetComponent<LetterKey>();
            if(key != null){
                hit.collider.GetComponent<LetterKey>().KeySelected();
            }
            
            EnterKey enter = hit.collider.GetComponent<EnterKey>();
            if(enter != null){
                hit.collider.GetComponent<EnterKey>().SubmitEntry();
            }
            
            ClearKey clear = hit.collider.GetComponent<ClearKey>();
            if(clear != null){
                hit.collider.GetComponent<ClearKey>().ClearText();
            }
        }
    }

}
