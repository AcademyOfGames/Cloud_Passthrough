using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SubmitButton : GenericVRClick
{
    public enum ButtonType { PlayAgain, Submit, Cancel}
    public ButtonType type;
    private FeedbackLogic feedback;
    
    public override void Click()
    {
        base.Click();
        FindObjectOfType<GoogleSheets>().AddEventData(type + " clicked.", SystemInfo.deviceUniqueIdentifier);

        switch (type)
        {
            case ButtonType.PlayAgain:
                SceneManager.LoadScene(0);

                break;

            case ButtonType.Submit:
                feedback.NextFeedbackPanel();

                break;

            case ButtonType.Cancel:
                feedback.CancelFeedback();
                break;
        }
    }


    private void Awake()
    {
        feedback = FindObjectOfType<FeedbackLogic>();
    }
}
