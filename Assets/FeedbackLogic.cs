using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FeedbackLogic : MonoBehaviour
{
    public GameObject[] feedbackPanels;
    [HideInInspector]
    public bool feedbackStarted;
    public bool feedbackEnded;
    public int currentFeedbackPanel = 0;

    public LineRenderer[] lasers;

    public TextMeshProUGUI emailInput;
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

    void ToggleLasers(bool on)
    {
        foreach (var l in lasers)
        {
            l.enabled = on;
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
    }

    public void CancelFeedback()
    {
        FindObjectOfType<GoogleSheets>().AddEventData("Feedback Cancelled", SystemInfo.deviceUniqueIdentifier);

        ToggleLasers(false);
        FindObjectOfType<BirdMovement>().ToggleControllerUI(true);

        gameObject.SetActive(false);

    }
    
    public void SetStarRating(int index)
    {
        FindObjectOfType<GoogleSheets>().AddEventData("Stars set to " + index, SystemInfo.deviceUniqueIdentifier);

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FeedbackEnded()
    {
        FindObjectOfType<BirdMovement>().ToggleControllerUI(true);
        ToggleLasers(false);
        feedbackEnded = true;
        gameObject.SetActive(false);
    }
}
