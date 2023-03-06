using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BrickLogic : MonoBehaviour
{
    private bool _activated;
    private bool _onFloor = true;

    private MeshRenderer renderer;

    private static bool brickCrashSound;

    private float timePassed = 0;
    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<MeshRenderer>();
        renderer.enabled = false;

       // Invoke(nameof(ActivateBrick),7.7f);
    }

    public void ActivateBrick()
    {
        _activated = true;

    }
    private void OnCollisionEnter(Collision other)
    {
        if (_activated && other.collider.gameObject.CompareTag("Tree") )
        {
           // StartCoroutine(nameof(TurnColliderOffAndOn));

            if (!brickCrashSound)
            {
                FindObjectOfType<TreeBehavior>().CrashThroughBricks();
                brickCrashSound = true;
            }
            renderer.enabled = true;
        }

        if (other.gameObject.CompareTag("floor") ||other.gameObject.CompareTag("Brick") )
        {
            _onFloor = true;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("floor")||other.gameObject.CompareTag("Brick") )
        {
            _onFloor = false;
        }    
    }


    // Update is called once per frame
    void Update()
    {
        if (transform.position.y > 5)
        {
            GetComponent<BoxCollider>().enabled = false;
        }
        else
        {
            GetComponent<BoxCollider>().enabled = true;

        }
    }

    IEnumerator TurnColliderOffAndOn()
    {
        yield return new WaitForSeconds(Random.Range(8f,10f));

        while (transform.position.y > 5)
        {
            GetComponent<BoxCollider>().enabled = false;
            yield return new WaitForSeconds(.6f); 
            GetComponent<BoxCollider>().enabled = true; 
            yield return new WaitForSeconds(Random.Range(.6f,1.8f));
        }
        
    }
}
