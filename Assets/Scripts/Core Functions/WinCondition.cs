using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinCondition : MonoBehaviour
{
    [Header("Layer Masks")]

    public LayerMask Player1;
    // public LayerMask Player2;


    [Header("Events")]

    public UnityEngine.Events.UnityEvent Player1Event;
    // public UnityEngine.Events.UnityEvent Player2Event;


    private void OnTriggerEnter(Collider other)
    {
        if ((Player1.value & (1 << other.transform.gameObject.layer)) > 0)
        {
            Player1Event.Invoke();
            Debug.Log("Player 1 Wins!!!");
        }

        // else if ((Player2.value & (1 << other.transform.gameObject.layer)) > 0)
        // {
            
            
        //     Debug.Log("Player 2 Wins!!!");
        // }
    }
}
