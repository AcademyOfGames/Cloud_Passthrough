using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackLogic : MonoBehaviour
{
    public GameObject[] feedbackPanels;
    [HideInInspector]
    public bool feedbackStarted;
    public bool feedbckEnded;
    public int currentFeedbackPanel = 0;

    public LineRenderer[] lasers;
    public void StartFeedback()
    {
        if (feedbackStarted) return;
        
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
        print("Setting to false: " + feedbackPanels[currentFeedbackPanel].name);
        feedbackPanels[currentFeedbackPanel].SetActive(false);
        currentFeedbackPanel++;
        if(currentFeedbackPanel > feedbackPanels.Length || feedbckEnded) return;
        feedbackPanels[currentFeedbackPanel].SetActive(true);
        print("Setting to true " + feedbackPanels[currentFeedbackPanel].name);
    }

    public void CancelFeedback()
    {
        ToggleLasers(false);
        FindObjectOfType<BirdMovement>().ToggleControllerUI(true);

        gameObject.SetActive(false);

    }
    
    public void SetStarRating(int index)
    {
        print("Stars set to " +  index);
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

        feedbckEnded = true;
        gameObject.SetActive(false);
    }
}
