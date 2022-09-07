using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmitButton : GenericVRClick
{
    private FeedbackLogic feedback;
    
    public override void Click()
    {
        base.Click();
        feedback.NextFeedbackPanel();
        
    }


    private void Awake()
    {
        feedback = FindObjectOfType<FeedbackLogic>();
    }
}
