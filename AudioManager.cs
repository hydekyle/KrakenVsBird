using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public AudioSource audioSource_speakers;
    Animator animator_speakers;

    private void Start()
    {
        animator_speakers = audioSource_speakers?.GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F8)) StartMusic();
    }

    public void StartMusic()
    {
        animator_speakers?.Play("moveUp");
        audioSource_speakers?.Play();
        CamaraManager.Instance.SetCameraMode(CamaraMode.Rave);
    }
}
