using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FeedbackLogic : MonoBehaviour
{
    public GameObject[] feedbackPanels;
    [HideInInspector] public bool feedbackStarted;

    private UICustomInteraction[] hands;
    public bool feedbackEnded;
    public int currentFeedbackPanel = 0;

    public LineRenderer[] lasers;

    public TextMeshProUGUI emailInput;

    private void Awake()
    {
        hands = FindObjectsOfType<UICustomInteraction>();
    }

    public void StartFeedback()
    {
        if (feedbackStarted) return;
        FindObjectOfType<GoogleSheets>().AddEventData("Feedback Started", SystemInfo.deviceUniqueIdentifier);

        FindObjectOfType<BirdMovement>().ToggleControllerUI(false);
        FindObjectOfType<BirdStateChanger>().SwitchState(BirdStateChanger.BirdState.TakeOff);

        GetComponent<Animator>().Play("UIFadeIn");
        feedbackStarted = true;
        feedbackPanels[0].SetActive(true);

        ToggleLasers(true);
    }

    private void ToggleLasers(bool on)
    {
        foreach (var hand in hands)
        {
            hand.ToggleLaser(on);
        }
    }


    public void NextFeedbackPanel()
    {
        feedbackPanels[currentFeedbackPanel].SetActive(false);
        currentFeedbackPanel++;
        if (currentFeedbackPanel > feedbackPanels.Length - 1 || feedbackEnded)
        {
            FindObjectOfType<GoogleSheets>().AddEventData(" Email Entered " + emailInput.text, SystemInfo.deviceUniqueIdentifier);
            FeedbackEnded();
            return;
        }
        feedbackPanels[currentFeedbackPanel].SetActive(true);
        print("Setting to true " + feedbackPanels[currentFeedbackPanel].name);
    }

    public void CancelFeedback()
    {
        FindObjectOfType<GoogleSheets>().AddEventData("Feedback Cancelled", SystemInfo.deviceUniqueIdentifier);
        FindObjectOfType<BirdMovement>().ToggleControllerUI(true);

        ToggleLasers(false);

        gameObject.SetActive(false);

    }
    
    public void SetStarRating(int index)
    {
        FindObjectOfType<GoogleSheets>().AddEventData("Stars set to " + index, SystemInfo.deviceUniqueIdentifier);

    }

    public void FeedbackEnded()
    {
        FindObjectOfType<BirdMovement>().ToggleControllerUI(true);
        ToggleLasers(false);
        feedbackEnded = true;
        gameObject.SetActive(false);
    }
}
