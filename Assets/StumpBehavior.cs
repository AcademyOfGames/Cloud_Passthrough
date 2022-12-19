using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

public class StumpBehavior : MonoBehaviour
{
    public Rigidbody[] fishRbs;

    public GameObject poofEffect;
    
    public GameObject prey;

    public Transform preySpawner;

    [HideInInspector] public int totalFishAlive = 3;

    [HideInInspector] public bool fishSystemOn;

    public GameObject fishBucket;
    public GameObject seedBucket;
    public BeeSystem beeSystem;

    public List<GameObject> fish;
    
    internal void DeactivateFishBucket()
    {
        FindObjectOfType<ControlUIManager>().ToggleEagleControlUI(false);
        fishSystemOn = false;
        fishBucket.SetActive(false);
        fishBucket.transform.SetParent(null);
        foreach (var f in fish)
        {
            if(f!=null) Destroy(f);
        }
        fish.Clear();
    }

    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<GoogleSheets>().AddEventData("Stump appeared", SystemInfo.deviceUniqueIdentifier);
        fish = new List<GameObject>();
    }

    public void ActivateFishBucket()
    {
        fishSystemOn = true;
        gameObject.SetActive(true); 
        fishBucket.SetActive(true);
        
        SetStump();
    }

    private void SetStump()
    {
        gameObject.transform.SetParent(null);
        fishBucket.transform.rotation = Quaternion.identity;
        fishBucket.transform.position =
            new Vector3(fishBucket.transform.position.x, 1.722f, fishBucket.transform.position.z);

        seedBucket.SetActive(false);
        seedBucket.transform.rotation = Quaternion.identity;
        seedBucket.transform.position =
            new Vector3(fishBucket.transform.position.x, 1.722f, fishBucket.transform.position.z);
    }

    public void SpawnMorePrey()
    {
        if (totalFishAlive >= 4 || !fishSystemOn) return;
        totalFishAlive++;
        GameObject newFish = Instantiate(prey, preySpawner.position, Quaternion.identity);
        newFish.transform.SetParent(gameObject.transform);
        newFish.GetComponent<Rigidbody>().mass = 1000;
        StartCoroutine(nameof(ResetFishMass),newFish);
        newFish.transform.eulerAngles = new Vector3(286.403046f, 283.090424f, 347.42511f);
        newFish.transform.localScale = new Vector3(1.59105301f,1.1302501f,1.33500004f);
        fish.Add(newFish);
    }
    public void ActivateBeeSystem()
    {
        DeactivateFishBucket();
        SetStump();
        gameObject.SetActive(true);
        beeSystem.gameObject.SetActive(true);
        seedBucket.SetActive(true);
    }
    IEnumerator ResetFishMass(GameObject obj)
    {
        yield return new WaitForSeconds(2f);
        obj.GetComponent<Rigidbody>().mass = 1;
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
    
    public void RemoveFish(GameObject obj)
    {
        totalFishAlive--;
        fish.Remove(obj);
        Destroy(obj);    
    }
}
