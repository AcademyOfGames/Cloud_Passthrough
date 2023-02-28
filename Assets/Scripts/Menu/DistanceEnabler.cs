using System.Collections.Generic;
using UnityEngine;

public class DistanceEnabler : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float radius = 3.0f;
    [SerializeField]List<Transform> components = new List<Transform>();
    bool shown = false;

    private void Start()
    {
        if (target == null)
        {
            // Find object with tag Player.
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }

        //Add children to components list if list is empty.
        if(components.Count == 0)
        {
            components = new List<Transform>();
            Transform[] transforms = GetComponentsInChildren<Transform>();
            foreach (Transform t in transforms)
            {
                if (t == transforms[0]) continue; //Ignore the root.
                components.Add(t);
            }
        }

        EnableComponents(false);
    }

    // target is inside threshold
    // target exits radius.
    private void Update()
    {
        float distance = Vector3.Distance(target.position, transform.position);
        if (distance <= radius)
        {
            if (shown) return;
            //show
            EnableComponents(true);
        }
        else
        {
            //hide
            if (!shown) return;
            EnableComponents(false);
        }
    }

    // Enables or disables all components on list.
    private void EnableComponents(bool enable)
    {
        foreach (Transform t in components)
        {
            t.gameObject.SetActive(enable);
        }
        shown = enable;
    }
}
