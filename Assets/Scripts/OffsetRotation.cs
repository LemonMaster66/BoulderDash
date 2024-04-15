using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffsetRotation : MonoBehaviour
{
    private Vector3 vectoffset;

    public GameObject Target;

    [SerializeField] private float speed = 3.0f;

    public bool Follow;


    void Start()
    {
        vectoffset = transform.position - Target.transform.position;
    }

    void Update()
    {
        if (Follow == true)
        {
            transform.position = Target.transform.position + vectoffset;
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, Target.transform.rotation, speed * Time.deltaTime);
    }
}