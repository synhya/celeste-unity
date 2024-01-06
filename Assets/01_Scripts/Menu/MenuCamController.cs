using System;
using UnityEngine;

public class MenuCamController : MonoBehaviour
{
    [SerializeField] private Transform pivotT;
    [SerializeField] private float rotateSpeed = 5;
    private Camera cam;
    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    private void Update()
    {
        transform.RotateAround(pivotT.position, Vector3.up, Time.deltaTime * rotateSpeed);


        if (Input.GetKeyDown(KeyCode.C))
        {
            // move to position 1. create buildings first
        }
    }

} 
