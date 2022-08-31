using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarLogic : MonoBehaviour
{
    public Toggle[] stars;

    public void UpdateStars(int starNum)
    {
        for (int i = 0; i < 5; i++)
        {
            stars[i].isOn = i < starNum;
        }
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
