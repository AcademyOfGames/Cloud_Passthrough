using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerBehavior : MonoBehaviour
{
    public Vector3 spawnPoint;
    public GameObject heroBee;
    
    static int totalFlowersPlanted = 0;

    public FlowerBehavior otherFlowers;

    public bool secondFlowersActive;
    // Start is called before the first frame update
    void Start()
    {
        totalFlowersPlanted++;
        if (totalFlowersPlanted == 3)
        {
            Invoke("SpawnHeroBee",2f);
        }
    }

    void SpawnHeroBee()
    {
        heroBee.SetActive(true);
    }
    public IEnumerator Grow(Vector3 pos)
    {
        gameObject.SetActive(true);

        var t = transform;
        var localScale = t.localScale;

        Vector3 originalScale = localScale;
        
        localScale = new Vector3(localScale.x,0,localScale.z);
        t.localScale = localScale;

        spawnPoint = pos;
        
        //add offset for the plant to grow upward
        pos.y -= 2;
        t.position = pos;
        
        float timePassed = 0;
        while (timePassed <= 0)
        {
            timePassed += Time.deltaTime * .5f;

            t.position = Vector3.Lerp(t.position, spawnPoint, timePassed );
            t.localScale = Vector3.Lerp(t.localScale, originalScale, timePassed );
            yield return new WaitForFixedUpdate();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<HeroBeeBehavior>() != null && secondFlowersActive)
        {
            IEnumerator grow = otherFlowers.Grow(otherFlowers.transform.position);
            StartCoroutine(grow);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
