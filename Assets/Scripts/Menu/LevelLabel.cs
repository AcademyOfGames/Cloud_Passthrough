using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component checks the distance from the target,
/// and sets the gameObject active or inactive depending the target distance.
/// </summary>
public class LevelLabel : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float radius = 3.0f;
    [SerializeField] int levelToLoad = 0;
    List<Transform> components;
    bool shown = false;

    private void Start()
    {
        if(target == null)
        {
            Debug.LogError("Missing target from LevelLabel Component");
        }

        components = new List<Transform>();
        Transform[] transforms = GetComponentsInChildren<Transform>();
        foreach(Transform t in transforms)
        {
            if (t == transforms[0]) continue; //Ignore the root.
            components.Add(t);
        }

        EnableComponents(false);
    }
    // target is inside threshold
    // target exits radius.
    private void Update()
    {
        float distance = Vector3.Distance(target.position, transform.position);
        if(distance <= radius)
        {
            if (shown) return; 
            //show
            Debug.Log("Enabling components");
            EnableComponents(true);
        }
        else
        {
            //hide
            if (!shown) return;
            Debug.Log("Disabling components");
            EnableComponents(false);
        }
    }

    private void EnableComponents(bool enable)
    {
        foreach(Transform t in components)
        {
            t.gameObject.SetActive(enable);
        }
        shown= enable;
    }

}
