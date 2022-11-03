using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveTrigger : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        print("Trigger entered "+other.name);
        if (other.gameObject.CompareTag("Deer"))
        {
            FindObjectOfType<DissolveControl>().StartDissolve();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
