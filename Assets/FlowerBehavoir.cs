using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerBehavoir : MonoBehaviour
{
    public float growAmount;
    public Vector3 spawnPoint;
    public GameObject heroBee;
    
    static int totalFlowersPlanted = 0;
    // Start is called before the first frame update
    void Start()
    {
        totalFlowersPlanted++;
        if (totalFlowersPlanted >= 3)
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
        Vector3 scale = transform.localScale;
        scale.y = 0;
        spawnPoint = pos;
        
        //add offset for the plant to grow upward
        spawnPoint.y += 2;
        var t = transform;
        t.position = pos;
        float timeLeft = 2f;
        while (timeLeft >=0f)
        {
            timeLeft -= Time.deltaTime;
            scale.y += growAmount;
            t.localScale = scale;
            
            
            t.position = Vector3.Lerp(t.position, spawnPoint, timeLeft );
            yield return new WaitForFixedUpdate();
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
