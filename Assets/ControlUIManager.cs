using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlUIManager : MonoBehaviour
{

    public GameObject leftEagleControls;
    public GameObject rightEagleControls;

    public GameObject fishControlUI;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void ToggleEagleControlUI(bool toggle)
    {
        FindObjectOfType<BirdStateChanger>().customControlsUnlocked = toggle;

        leftEagleControls.SetActive(toggle);
        rightEagleControls.SetActive(toggle);
        fishControlUI.SetActive(toggle);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
