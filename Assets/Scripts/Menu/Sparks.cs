using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sparks : MonoBehaviour
{
    ParticleSystem particle;

    private void OnEnable()
    {
        particle = GetComponent<ParticleSystem>();
    }

    private void LateUpdate()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 0.1f);
        if(colliders.Length >= 1)
        {
            // Play particle if object is in contact with fire and particle is not playing already.
            foreach(Collider col in colliders)
            {
                if (!col.CompareTag("fire")) continue;

                if (!particle.isPlaying)
                {
                    particle.Play();
                }      
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.1f);
    }
}
