using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionForce : MonoBehaviour
{
    // public float Force;
    // public float Radius;

    // public void Boom()
    // {
    //     Collider[] colliders = Physics.OverlapSphere(transform.position, Radius);

    //     foreach (Collider nearby in colliders)
    //     {
    //         Rigidbody rb = nearby.GetComponent<Rigidbody>();
    //         if (rb != null)
    //         {
    //             rb.AddExplosionForce(Force, transform.position, Radius);
    //         }
    //     }
    // }

    public float radius = 5.0f;
    public float power = 10.0f;
    public float falloff = 1.0f;

    void OnDrawGizmosSelected()
    {
        // Draw the explosion radius in the scene window
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    public void Boom()
    {
        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Calculate the distance from the center of the explosion
                float distance = (rb.transform.position - explosionPos).magnitude;

                // Calculate the explosion force at the current distance
                float force = power * (1.0f - (distance / radius));
                force = Mathf.Max(force, 0.0f);  // Clamp the force to zero if it's negative

                // Apply the explosion force
                rb.AddExplosionForce(force, explosionPos, radius, falloff);
            }
        }
    }

    public void JumpPad()
    {
        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Apply the explosion force
                rb.AddForce(0.0f, power, 0.0f);
            }
        }
    }
}
