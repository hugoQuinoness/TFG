using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2 : MonoBehaviour
{
    public static Boss2 Instance;

    public Cinemachine.CinemachineVirtualCamera virtualCamera;

    private Animator animator;

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

        animator = GetComponent<Animator>();

        virtualCamera = GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>();

    }

    public void ChangeCameraPriority()
    {
        if (CMCameraManager.Instance.currentCinemachineVCamera == null)
        {
            CMCameraManager.Instance.currentCinemachineVCamera = virtualCamera;
        }

        CMCameraManager.Instance.ChangeVcamPriority(virtualCamera);
    }

    public void PlayAnimation(string animation)
    {
        animator.Play(animation);
    }
}
