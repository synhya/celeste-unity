using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeManager : MonoBehaviour
{
    public static SmokeManager Instance => instance;
    private static SmokeManager instance = null;
    
    [SerializeField] private GameObject psPopSmokeObj;
    [SerializeField] private GameObject psSlideSmokeObj;

    private float popTime;
    
    void Awake() 
    {
        if (instance == null)
        {
            instance = this;

            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        popTime = psPopSmokeObj.GetComponent<ParticleSystem>().main.startLifetime.constantMax;
    }

    public void CreatePopSmoke(Vector2 posWS) =>
        Destroy(Instantiate(psPopSmokeObj, posWS, Quaternion.identity), popTime);
} 
