using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerBehavoir : MonoBehaviour
{
    public float growAmount;

    public float upMoveAmount;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(nameof(Grow));
    }

    private IEnumerator Grow()
    {
        Vector3 scale = Vector3.one;
        scale.y = 0;
        var t = transform;

        Vector3 pos = t.position;
        float timeLeft = 2f;
        while (timeLeft >=0f)
        {
            timeLeft -= Time.deltaTime;
            scale.y += growAmount;
            t.localScale = scale;
            
            pos.y += upMoveAmount;
            t.position = pos;
            yield return new WaitForFixedUpdate();
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
