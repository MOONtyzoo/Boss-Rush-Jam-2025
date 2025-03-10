using UnityEngine;

public class PlayerWallState : State<PlayerController>
{
    private PrototypePlayerDataSO playerData;
    private InputReaderSO inputReader;

    protected override void Enter()
    {
        playerData = RunnerObject.GetPlayerData();
        inputReader = RunnerObject.GetInputReader();

        if (RunnerObject.GetVelocityY() < 0) { RunnerObject.SetVelocityY(0f); }
        RunnerObject.InvokeOnWallGrabStarted();
    }

    public override void FixedTick(float fixedDeltaTime) {
        ApplyWallFriction();
    }

    public override void Tick(float deltaTime) {}

    public override void HandleStateTransitions() {
        if (RunnerObject.IsGrounded()) {
            RunnerObject.SetState(PlayerController.PlayerStates.Grounded);
            return;
        }

        if (inputReader.GetInputJumpDown() && RunnerObject.CanWallJump()) {
            RunnerObject.WallJump();
            RunnerObject.SetState(PlayerController.PlayerStates.Aerial);
            return;
        }

        if (!RunnerObject.IsPushingAgainstWall()) {
            RunnerObject.SetState(PlayerController.PlayerStates.Aerial);
            return;
        }
    }

    public override void Exit() {
        RunnerObject.InvokeOnWallGrabEnded();
    }

    private void ApplyWallFriction() {
        float velocityChange = -Mathf.Sign(RunnerObject.GetVelocityY()) * playerData.wallFriction * Time.deltaTime;
        float newVelocityY = RunnerObject.GetVelocityY() + velocityChange;

        if (Mathf.Abs(velocityChange) < Mathf.Abs(RunnerObject.GetVelocityY())) {
            RunnerObject.SetVelocityY(newVelocityY);
        } else {
            RunnerObject.SetVelocityY(0f);
        }
    }
}
