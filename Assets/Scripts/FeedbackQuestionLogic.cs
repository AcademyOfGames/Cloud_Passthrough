using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackQuestionLogic : MonoBehaviour
{
    public GameObject keys;

    private void OnEnable()
    {
        keys.SetActive(true);
    }

    private void OnDisable()
    {
        keys.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
