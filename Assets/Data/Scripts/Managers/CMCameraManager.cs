using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMCameraManager : MonoBehaviour
{
    public static CMCameraManager Instance;

    public Cinemachine.CinemachineVirtualCamera currentCinemachineVCamera;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }



    public void ChangeVcamPriority(Cinemachine.CinemachineVirtualCamera vCam)
    {
        
            if(currentCinemachineVCamera != null)
            {
                currentCinemachineVCamera.Priority = 0;
            }

            currentCinemachineVCamera = vCam;

            vCam.Priority = 1;
    }


}
