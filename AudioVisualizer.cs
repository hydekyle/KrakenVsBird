using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioVisualizer : MonoBehaviour
{
    AudioSource audioSource;
    public Transform speakerLeft;
    public Transform speakerRight;
    private float mainScale;
    public float scaleMultiplier = 10f;
    public int spectrumSelector = 30;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        mainScale = speakerLeft.localScale.x;
    }

    private void Update()
    {
        float[] spectrum = new float[256];
        audioSource.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);
        SetSpeakers(spectrum[spectrumSelector]);
    }

    void SetSpeakers(float spectrumValue)
    {
        Vector3 targetScale = Vector3.one * Mathf.Clamp(mainScale * spectrumValue * scaleMultiplier * 8, 40f, 100f);
        speakerLeft.localScale = Vector3.Lerp(speakerLeft.localScale, targetScale, Time.deltaTime * 4);
        speakerRight.localScale = Vector3.Lerp(speakerRight.localScale, targetScale, Time.deltaTime * 4);
    }
}
