using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerBehavoir : MonoBehaviour
{
    public float growAmount;
    public Transform spawnPoint;
    public GameObject heroBee;
    
    static int totalFlowersPlanted = 0;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(nameof(Grow));
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
    private IEnumerator Grow()
    {
        Vector3 scale = Vector3.one;
        scale.y = 0;
        var t = transform;
        spawnPoint.position -= Vector3.right * 5f;
        Vector3 pos = t.position;
        float timeLeft = 2f;
        while (timeLeft >=0f)
        {
            timeLeft -= Time.deltaTime;
            scale.y += growAmount;
            t.localScale = scale;
            
            
            t.position = Vector3.Lerp(t.position, spawnPoint.position, timeLeft /2f);
            yield return new WaitForFixedUpdate();
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
