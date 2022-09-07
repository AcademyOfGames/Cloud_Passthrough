using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarButton : GenericVRClick
{
    public int starIndex;
    public bool isOn;
    public Sprite onStar;
    public Sprite offStar;

    public Image starImage;

    public StarButton[] starGroup;

    private FeedbackLogic feedback;

    public void ToggleStar(bool on)
    {
        isOn = on;
        starImage.sprite = isOn ? onStar : offStar;
    }

    public override void Click()
    {
        foreach (var star in FindObjectsOfType<StarButton>())
        {
            star.ToggleStar(false);
        }
        feedback.SetStarRating(starIndex);
        if (isOn)
        {
            foreach (var star in starGroup)
            {
                star.ToggleStar(false);
            }
        }
        else
        {
            foreach (var star in starGroup)
            {
                star.ToggleStar(true);
            }           
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        feedback = FindObjectOfType<FeedbackLogic>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
