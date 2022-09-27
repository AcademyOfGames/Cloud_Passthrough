using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class StumpBehavior : MonoBehaviour
{
    public Rigidbody[] fishRbs;

    public GameObject poofEffect;
    
    public GameObject prey;

    public Transform preySpawner;

    [HideInInspector]
    public int totalFishAlive = 3;

    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<GoogleSheets>().AddEventData("Stump appeared", SystemInfo.deviceUniqueIdentifier);
    }

    public void SpawnMorePrey()
    {
        if (totalFishAlive >= 4) return;
        totalFishAlive++;
        GameObject newFish = Instantiate(prey, preySpawner.position, Quaternion.identity);
        newFish.transform.SetParent(gameObject.transform);
        newFish.GetComponent<Rigidbody>().mass = 1000;
        StartCoroutine(nameof(ResetFishMass),newFish);
        newFish.transform.eulerAngles = new Vector3(286.403046f, 283.090424f, 347.42511f);
        newFish.transform.localScale = new Vector3(1.59105301f,1.1302501f,1.33500004f);

    }

    IEnumerator ResetFishMass(GameObject fish)
    {
        yield return new WaitForSeconds(2f);
        fish.GetComponent<Rigidbody>().mass = 1;

    }
    private void OnCollisionEnter(Collision other)
    {
        if (!other.gameObject.CompareTag("floor")) return;
        foreach (var rb in fishRbs)
        {
            if (rb == null) continue;
            rb.useGravity = true;
            rb.isKinematic = false;
        }
        poofEffect.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
