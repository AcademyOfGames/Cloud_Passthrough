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
        new Cognitive3D.CustomEvent($"{type} clicked");

        switch (type)
        {
            case ButtonType.PlayAgain:
                FindObjectOfType<GoogleSheets>().AddEventData(" Email Entered " + feedback.emailInput.text, SystemInfo.deviceUniqueIdentifier);
                new Cognitive3D.CustomEvent($"Email entered. {feedback.emailInput.text}");

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
