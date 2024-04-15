using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoltenEmission : MonoBehaviour
{
    public Rigidbody rb;

    public Material EmissiveMaterial;

    public float XEmission;
    public float ZEmission;

    [Tooltip("How Gradualy the Emission Increases *150 Default*")]
    public int EmissionDivide = 150;

    // Update is called once per frame
    void FixedUpdate()
    {


        if (rb.velocity.x > 0)
            {
                XEmission = rb.velocity.x*-1;
            }
            else
            {
                XEmission = rb.velocity.x;
            }


            if (rb.velocity.z > 0)
            {
                ZEmission = rb.velocity.z*-1;
            }
            else
            {
                ZEmission = rb.velocity.z;
            }

            EmissiveMaterial.SetFloat("_EmissiveExposureWeight", (XEmission/EmissionDivide) + (ZEmission/EmissionDivide)+1);

    }
}
