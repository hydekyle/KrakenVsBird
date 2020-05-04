using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CamaraMode { Idle, Rave }
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
        switch (mode)
        {
            case CamaraMode.Rave: mainCamera.GetComponent<Animator>().Play("MusicOn"); break;
        }
    }
}
