using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CancelButton : GenericVRClick
{
    private FeedbackLogic feedback;
    
    public override void Click()
    {
        base.Click();
        feedback.CancelFeedback();
        
    }


    private void Awake()
    {
        feedback = FindObjectOfType<FeedbackLogic>();
    }
}