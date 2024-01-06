using System;
using System.Linq;
using Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

public class MenuCineCamController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera menuCam;
    [SerializeField] private CinemachineVirtualCamera level1Cam;
    
    private bool selectedLevel = false;

    private void Update()
    {
 
        if (Input.GetKeyDown(KeyCode.C) && !selectedLevel)
        {
            selectedLevel = true;
            menuCam.Priority = 0;
            level1Cam.Priority = 1;
        }

        else if (Input.GetKeyDown(KeyCode.X) && selectedLevel)
        {
            selectedLevel = false;
            menuCam.Priority = 1;
            level1Cam.Priority = 0;
        }
    }
} 
