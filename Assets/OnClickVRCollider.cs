using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnClickVRCollider : MonoBehaviour
{
    //Public Variables:
    private Button btn;
    
    public void Click()
    {
        btn.onClick.Invoke();
    }
}
