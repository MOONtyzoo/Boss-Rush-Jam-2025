using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName="ScriptableObjects/InputReaderSO", fileName="InputReaderSO")]
public class InputReaderSO : ScriptableObject
{
    private GameInput gameInput;

    private void OnEnable() {
        gameInput = new GameInput();
        gameInput.Player.Enable();
    }

    private void OnDisable() {
        gameInput.Player.Disable();
    }

    public Vector2 GetInputMove() {
       return gameInput.Player.Move.ReadValue<Vector2>();
    }

    public float GetInputMoveX() {
        return gameInput.Player.Move.ReadValue<Vector2>().x;
    }

    public float GetInputMoveY() {
        return gameInput.Player.Move.ReadValue<Vector2>().y;
    }

    public bool GetInputCrouch() {
        if (Gamepad.current == null) {
            return GetInputMoveY() < 0f;
        } else {
            return GetInputMoveY() <= -0.85f;
        }
    }

    public Vector2 GetInputAimPosition(Transform aimOrigin) {
        if (Gamepad.current == null) {
            return GetMouseWorldPosition();
        } else {
            return (Vector2)aimOrigin.position + gameInput.Player.Aim.ReadValue<Vector2>();
        }
    }

    public bool GetInputJump() {
        return gameInput.Player.Jump.IsPressed();
    }

    public bool GetInputJumpDown() {
        return gameInput.Player.Jump.WasPerformedThisFrame();
    }

    public bool GetInputShootDown() {
        return gameInput.Player.Shoot.WasPerformedThisFrame();
    }

    public bool GetInputGrappleDown() {
        return gameInput.Player.Grapple.WasPerformedThisFrame();
    }

    public bool GetInputPauseDown() {
        return gameInput.Player.Pause.WasPerformedThisFrame();
    }

    private Vector2 GetMouseWorldPosition() => Camera.main.ScreenToWorldPoint(Input.mousePosition);
}

