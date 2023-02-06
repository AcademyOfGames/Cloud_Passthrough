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
            Invoke(nameof(WaitAndDeactivate), time);
        }
    }

    void WaitAndDeactivate()
    {
        gameObject.SetActive(false);
    }
}
