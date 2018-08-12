﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    // Transform of the camera to shake. Grabs the gameObject's transform
    // if null.
    private Transform camTransform;

    // How long the object should shake for.
    public float shakeDuration = 0f;

    // Amplitude of the shake. A larger value shakes the camera harder.
    public float shakeAmount = 0.3f;
    public float decreaseFactor = 1.0f;


    void Awake()
    {
        camTransform = transform;
    }

    public void Shake(float duration)
    {
        shakeDuration = duration;
    }

    void Update()
    {
        if (shakeDuration > 0)
        {
            camTransform.localPosition = camTransform.transform.position + Random.insideUnitSphere * shakeAmount;
            shakeDuration -= Time.deltaTime * decreaseFactor;
        }
        else
        {
            shakeDuration = 0f;
        }
    }
}
