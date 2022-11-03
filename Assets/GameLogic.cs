using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    public TextMeshProUGUI vsText;
    
    // Start is called before the first frame update
    void Start()
    {
    }

    public void StartMatch(string selection)
    {
        vsText.text = selection;
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
