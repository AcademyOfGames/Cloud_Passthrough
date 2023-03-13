using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FeedbackLogic : MonoBehaviour
{
    public GameObject[] feedbackPanels;
    public GameObject playAgainPanel;
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

            CancelFeedback();
            return;
        }
        feedbackPanels[currentFeedbackPanel].SetActive(true);
    }
   
    public void TurnOnPlayAgainPanel()
    {
        playAgainPanel.SetActive(true);
    }

    public void CancelFeedback()
    {
        if (playAgainPanel.activeSelf)
        {
            FindObjectOfType<GoogleSheets>().AddEventData("Feedback Cancelled or ended", SystemInfo.deviceUniqueIdentifier);

            if(emailInput.text != "")
            {
                FindObjectOfType<GoogleSheets>().AddEventData(" Email Entered " + emailInput.text, SystemInfo.deviceUniqueIdentifier);
            }
                
            
            feedbackEnded = true;

            ToggleLasers(false);
            //FindObjectOfType<BirdMovement>().ToggleControllerUI(true);
            gameObject.SetActive(false);
        }
        else
        {
            TurnOnPlayAgainPanel();
        }


    }
    
    public void SetStarRating(int index)
    {
        FindObjectOfType<GoogleSheets>().AddEventData("Stars set to " + index, SystemInfo.deviceUniqueIdentifier);

    }


    public IEnumerator WaitAndStartFeedback()
    {
        yield return new WaitForSeconds(10);
        StartFeedback();
    }
}
