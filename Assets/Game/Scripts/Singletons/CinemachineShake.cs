using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

/*
    https://www.youtube.com/watch?v=ACf1I27I6Tk&t=337s

    Created following this tutorial for camera shake
*/

public class CinemachineShake : MonoBehaviour
{
    public static CinemachineShake Instance {get; private set;}

    private CinemachineBasicMultiChannelPerlin cinemachinePerlin;
    private float shakeTimer;

    void Awake() {
        if (Instance == null) {
            Instance = this;
            cinemachinePerlin = GetComponent<CinemachineBasicMultiChannelPerlin>();
        } else {
            Destroy(gameObject);
        }
        StopShake();
    }

    public void ShakeCamera(float intensity, float time) {        
        cinemachinePerlin.AmplitudeGain = intensity;
        shakeTimer = time;
    }

    public void StopShake() {
        cinemachinePerlin.AmplitudeGain = 0;
        shakeTimer = 0;
    }

    private void Update() {
        if (shakeTimer > 0) {
            shakeTimer -= Time.deltaTime;
            if (shakeTimer <= 0) {
                StopShake();
            }
        }
    }
}

