using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerBehavior : MonoBehaviour
{
    private Wind wind;
    public Vector3 spawnPoint;
    public GameObject heroBee;
    
    static int totalFlowersPlanted = 0;

    public FlowerBehavior otherFlowers;

    public static bool secondFlowersActive;
    static int totalBLoomingFlowers;

    bool secondFlowerBloom;
    public GameObject flowerIcon;
    static bool beeSoundtrackStarted;

    ControlUIManager controlUIManager;
    // Start is called before the first frame update
    void Start()
    {
        wind = FindObjectOfType<Wind>();
        controlUIManager = FindObjectOfType<ControlUIManager>();

        totalFlowersPlanted++;
        if (totalFlowersPlanted == 3)
        {
            Invoke("SpawnHeroBee",2f);
            controlUIManager.TurnOnSeedControls(false);

        }
    }

    void SpawnHeroBee()
    {
        heroBee.transform.position = transform.position;
        heroBee.SetActive(true);
    }
    public IEnumerator Grow(Vector3 pos)
    {
        if (!beeSoundtrackStarted)
        {
            beeSoundtrackStarted=true;
            FindObjectOfType<SoundtrackPlayer>().PlaySound("beeSong");
        }
        gameObject.SetActive(true);

        var t = transform;
        var localScale = t.localScale;

        Vector3 originalScale = localScale;
        
        localScale = new Vector3(localScale.x,0,localScale.z);
        t.localScale = localScale;

        spawnPoint = pos;
        
        //add offset for the plant to grow upward
        pos.y -= 1;
        t.position = pos;
        
        float timePassed = 0;
        while (timePassed <= 1)
        {
            timePassed += Time.deltaTime * .1f;

            t.position = Vector3.Lerp(t.position, spawnPoint, timePassed );
            t.localScale = Vector3.Lerp(t.localScale, originalScale, timePassed );
            yield return new WaitForFixedUpdate();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<HeroBeeBehavior>() != null && secondFlowersActive)
        {
            if (secondFlowerBloom) return;
            flowerIcon.SetActive(false);
            IEnumerator grow = otherFlowers.Grow(otherFlowers.transform.position);
            StartCoroutine(grow);

            totalBLoomingFlowers++;
            secondFlowerBloom = true;
            if (totalBLoomingFlowers== 3)
            {
                wind.StartRain();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivateSecondFlowers()
    {
        secondFlowersActive = true;
        flowerIcon.SetActive(true);
    }
}
