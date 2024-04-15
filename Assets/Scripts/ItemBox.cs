using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBox : MonoBehaviour
{

    //public GameObject ItemBoxPrefab;
    //public GameObject ItemStorage;

    private int RandomPick;
    

    [Header("Rocket Event")]
    public GameObject RocketSpawnPrefab;
    public Transform RocketSpawnData;

    public UnityEngine.Events.UnityEvent RocketEvent;


    [Header("Triple Rocket Event")]
    public GameObject TripleRocketSpawnPrefab;
    public Transform TripleRocketSpawnData;

    public UnityEngine.Events.UnityEvent TripleRocketEvent;

    [Header("Heat Seeking Rocket Event")]
    public UnityEngine.Events.UnityEvent HSRocketEvent;

    [Header("Fungus Event")]
    public UnityEngine.Events.UnityEvent FungusEvent;

    [Header("Bomb Event")]
    public UnityEngine.Events.UnityEvent BombEvent;

    [Header("Shock Wave Event")]
    public UnityEngine.Events.UnityEvent ShockEvent;

    [Header("Goop Event")]
    public UnityEngine.Events.UnityEvent GoopEvent;

    [Header("Shrink Ray Event")]
    public UnityEngine.Events.UnityEvent ShrinkEvent;

    [Header("Anti Gravity Event")]
    public UnityEngine.Events.UnityEvent GravityEvent;

    [Header("Moon Orbit Event")]
    public UnityEngine.Events.UnityEvent MoonEvent;


    public void RandomDraw()
    {
        int RandomPick = Random.Range(1,11);
        Debug.Log(RandomPick);

        if (RandomPick == 1)
        {
            Debug.Log("Rocket");
            RocketEvent.Invoke();
        }
        if (RandomPick == 2)
        {
            Debug.Log("Triple Rocket");
            TripleRocketEvent.Invoke();
        }
        if (RandomPick == 3)
        {
            Debug.Log("Heat Seeking Rocket");
            HSRocketEvent.Invoke();
        }
        if (RandomPick == 4)
        {
            Debug.Log("Fungus");
            FungusEvent.Invoke();
        }
        if (RandomPick == 5)
        {
            Debug.Log("Bomb");
            BombEvent.Invoke();
        }
        if (RandomPick == 6)
        {
            Debug.Log("Shock Wave");
            ShockEvent.Invoke();
        }
        if (RandomPick == 7)
        {
            Debug.Log("Goop");
            GoopEvent.Invoke();
        }
        if (RandomPick == 8)
        {
            Debug.Log("Shrink Ray");
            ShrinkEvent.Invoke();
        }
        if (RandomPick == 9)
        {
            Debug.Log("Anti Gravity");
            GravityEvent.Invoke();
        }
        if (RandomPick == 10)
        {
            Debug.Log("Moon");
            MoonEvent.Invoke();
        }
    }

    public void SpawnRocket()
    {
        Instantiate(RocketSpawnPrefab, RocketSpawnData.position, RocketSpawnData.rotation);
        RocketSpawnPrefab.transform.Rotate(90,0,0);
    }

    public void SpawnTripleRocket()
    {
        Instantiate(TripleRocketSpawnPrefab, TripleRocketSpawnData.position, TripleRocketSpawnData.rotation);
        TripleRocketSpawnPrefab.transform.Rotate(90,0,0);
    }
}
