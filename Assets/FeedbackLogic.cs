using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackLogic : MonoBehaviour
{
    public GameObject[] feedbackPanels;
    [HideInInspector]
    public bool feedbackStarted;
    public int currentFeedbackPanel = 0;
    public void StartFeedback()
    {
        if (feedbackStarted) return;
        feedbackStarted = true;
        feedbackPanels[0].SetActive(true);
    }

    public void NextFeedbackPanel()
    {
        feedbackPanels[currentFeedbackPanel].SetActive(false);
        currentFeedbackPanel++;
        feedbackPanels[currentFeedbackPanel].SetActive(true);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}