using System;
using System.Collections;
using UnityEngine;

public class RoomSpinner : MonoBehaviour
{
    public static RoomSpinner Instance {get; private set;}

    [SerializeField] private bool debugMode = false;
    
    public event Action OnRoomSpinStarted;
    public event Action OnRoomSpinEnded;

    private float rotationAngle = 0;
    private bool isSpinning = false;

    public enum SpinDirection {
        Clockwise90,
        Clockwise180,
        CounterClockwise90,
        CounterClockwise180,
    }

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Debug.Log("RoomSpinner is a Singleton! There may only be one of it in a scene.");
            Destroy(gameObject);
        }
    }

    public void SpinRoom(SpinDirection spinDirection) {
        float targetAngle = rotationAngle;
        switch(spinDirection) {
            case SpinDirection.Clockwise90: targetAngle -= 90; break;
            case SpinDirection.Clockwise180: targetAngle -= 180; break;
            case SpinDirection.CounterClockwise90: targetAngle += 90; break;
            case SpinDirection.CounterClockwise180: targetAngle += 180; break;
        }
        AudioManager.PlaySound(SoundEffects.RoomRotation);
        StartCoroutine(SpinRoomAnimationCoroutine(targetAngle));
    }

    private IEnumerator SpinRoomAnimationCoroutine(float targetAngle) {
        float spinDuration = 1.5f;
        float spinTimer = 0.0f;
        float spinSpeed = 6f;

        if (debugMode) Debug.Log("Spinning started!");
        isSpinning = true;
        OnRoomSpinStarted?.Invoke();
        Time.timeScale = 0f;
        CinemachineShake.Instance.ShakeCamera(5f, spinDuration);  
        while (spinTimer < spinDuration) {
            float newAngle = Mathf.Lerp(rotationAngle, targetAngle, spinSpeed*Time.unscaledDeltaTime);
            SetRotationAngle(newAngle);
            spinTimer += Time.unscaledDeltaTime;
            yield return null;
        }
        SetRotationAngle(targetAngle);
        if (debugMode) Debug.Log($"Spinning ended!");
        if (debugMode) Debug.Log($"New orientation: {GetOrientation()}");
        isSpinning = false;
        OnRoomSpinEnded?.Invoke();
        Time.timeScale = 1f;
        CinemachineShake.Instance.StopShake();  
    }

    private void SetRotationAngle(float newAngle) {
        rotationAngle = newAngle;
        transform.eulerAngles = new Vector3(0, 0, newAngle);
    }

    public Orientation GetOrientation() => OrientationUtilities.GetOrientationOfTransform(transform);
    public bool IsSpinning() => isSpinning;
}
