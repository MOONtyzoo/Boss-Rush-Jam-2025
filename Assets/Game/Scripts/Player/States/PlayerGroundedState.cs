using UnityEngine;

public class PlayerGroundedState : State<PlayerController>
{
    private PrototypePlayerDataSO playerData;
    private InputReaderSO inputReader;

    bool isCrouching = false;

    protected override void Enter()
    {
        playerData = RunnerObject.GetPlayerData();
        inputReader = RunnerObject.GetInputReader();

        RunnerObject.RefreshJumps();
    }
    public override void FixedTick(float fixedDeltaTime)
    {
        float targetVelocityX = playerData.moveSpeed * inputReader.GetInputMoveX();
        float acceleration = playerData.PickMoveAccelerationValue(isGrounded:true, targetVelocityX);

        HandleCrouchInput();
        if (isCrouching) {
            targetVelocityX = 0f;
            acceleration = 0.3f*playerData.PickMoveAccelerationValue(isGrounded:true, targetVelocityX);
        }

        RunnerObject.MoveX(targetVelocityX, acceleration);
    }

    public override void Tick(float deltaTime) {
        if (inputReader.GetInputJumpDown() && RunnerObject.CanJump()) {
            RunnerObject.Jump();
        }
    }

    public override void HandleStateTransitions() {
        if (!RunnerObject.IsGrounded()) {
            RunnerObject.SetState(PlayerController.PlayerStates.Aerial);
        }
    }

    public override void Exit() {
        isCrouching = false;
        RunnerObject.InvokeOnCrouchEnded();
    }

    private void HandleCrouchInput() {
        if (inputReader.GetInputCrouch() != isCrouching) {
            if (inputReader.GetInputCrouch() == true) {
                RunnerObject.InvokeOnCrouchStarted();
            } else {
                RunnerObject.InvokeOnCrouchEnded();
            }
        }
        isCrouching = inputReader.GetInputCrouch();
    }
}
