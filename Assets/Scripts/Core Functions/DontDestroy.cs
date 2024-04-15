using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    void Start()
    {
        
        for (int i = 0; i < Object.FindObjectsOfType<DontDestroy>().Length; i++)
        {
            DontDestroyOnLoad(Object.FindObjectsOfType<DontDestroy>()[i]);
        }
        
    }
}
