using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSystem : MonoBehaviour
{

    [Header("Menu Components")]
    [Header("Fire")]
    [SerializeField] FireBehaviour bonfire;

    [SerializeField] List<Transform> menuObjects = new List<Transform>();

    [SerializeField] Transform parentObjects;
    [SerializeField] Transform triggerParent;

    bool isEagleOn = false;
    BirdMovement eagle;
    
    private void OnEnable()
    {
        bonfire.onToggleBonfire.AddListener(DisplayMenu);
    }

    private void OnDisable()
    {
        bonfire.onToggleBonfire.RemoveListener(DisplayMenu);
    }

    private void Start()
    {
        eagle = FindObjectOfType<BirdMovement>();
        eagle.gameObject.SetActive(false);
        //if(bonfire != null) menuObjects.Add(bonfire.transform);
        ShowAllMenuObjects(false);
        StartFire();

    }

    private void DisplayMenu()
    {
        
        if (bonfire.IsOn)
        {
            Debug.Log("Displaying Menu");
            StartCoroutine(MenuSequence(true, 0.05f, null));
        }
        else
        {
            Debug.Log("Hiding Menu");
            StartCoroutine(MenuSequence(false, 0.05f, null));
        }
    }

    /// <summary>
    /// Sequence to hide or display Menu Game Objects.
    /// </summary>
    /// <param name="show"></param>
    /// <returns></returns>
    private IEnumerator MenuSequence(bool show, float delay, List<Transform> exceptions)
    {
        yield return new WaitForSeconds(delay);
        if(show)
            SetAllInvisible();
        foreach (Transform t in menuObjects)
        {
            if (IsInList(t, exceptions)) continue;
            t.gameObject.SetActive(true);
            Dissolve[] dissolves = t.GetComponentsInChildren<Dissolve>(true);
            foreach(Dissolve d in dissolves)
            {
                if (show)
                {
                    Debug.Log("Fade In" + d.transform.name);
                    StartCoroutine(d.FadeIn(0.5f));
                    if (!isEagleOn) eagle.gameObject.SetActive(true); // only turn on eagle the first time the menu displays.
                }

                else
                {
                    Debug.Log("Fade Out" + d.transform.name);
                    StartCoroutine(d.FadeOut());
                }
            }
            yield return new WaitForFixedUpdate();
        }
        parentObjects.gameObject.SetActive(show);
        triggerParent.gameObject.SetActive(show);
    }

    public void StartFire()
    {
        bonfire.gameObject.SetActive(true);
        bonfire.SetFire(false);
    }

    /// <summary>
    /// Sets active or inactive all GameObjects in menuObjects list.
    /// </summary>
    /// <param name="display"></param>
    private void ShowAllMenuObjects(bool display)
    {
        Debug.Log("Set all inactive");
        //parentObjects.gameObject.SetActive(display);
        foreach(Transform t in menuObjects)
        {
            t.gameObject.SetActive(display);
        }
    }

    private void SetAllInvisible()
    {
        foreach (Transform t in menuObjects)
        {
            foreach (Dissolve d in t.GetComponentsInChildren<Dissolve>())
            {
                d.SetInvisible();
            }
        }
    }

    private bool IsInList(Transform transform, List<Transform> list)
    {
        if (list == null) return false;
        foreach(Transform t in list)
        {
            if (t == transform)
            {
                return true;
            }
                
        }
        return false;
    }

}
