using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CamaraMode { Idle, Rave, StartGame }
public class CamaraManager : MonoBehaviour
{
    public static CamaraManager Instance;
    public Camera mainCamera;

    private void Start()
    {
        Instance = Instance ?? this;
        mainCamera = Camera.main;
    }

    public void SetCameraMode(CamaraMode mode)
    {
        Animator cameraAnimator = mainCamera.GetComponent<Animator>();
        switch (mode)
        {
            case CamaraMode.Rave: cameraAnimator.Play("MusicOn"); break;
            case CamaraMode.StartGame: cameraAnimator.Play("StartGame"); break;
            default: cameraAnimator.Play("Idle"); break;
        }
    }
}
