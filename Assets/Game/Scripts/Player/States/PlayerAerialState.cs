using UnityEngine;

public class PlayerAerialState : State<PlayerController>
{
    private PrototypePlayerDataSO playerData;
    private InputReaderSO inputReader;

    protected override void Enter()
    {
        playerData = RunnerObject.GetPlayerData();
        inputReader = RunnerObject.GetInputReader();
    }

    public override void FixedTick(float fixedDeltaTime) {
        float targetVelocityX = playerData.moveSpeed * inputReader.GetInputMoveX();
        float acceleration = playerData.PickMoveAccelerationValue(isGrounded:false, targetVelocityX);

        RunnerObject.MoveX(targetVelocityX, acceleration);
    }

    public override void Tick(float deltaTime) {
        if (inputReader.GetInputJumpDown() && RunnerObject.CanWallJump()) {
            RunnerObject.WallJump();
        }
    }

    public override void HandleStateTransitions() {
        if (RunnerObject.IsGrounded()) {
            RunnerObject.SetState(PlayerController.PlayerStates.Grounded);
            return;
        }

        if (RunnerObject.IsPushingAgainstWall()) {
            RunnerObject.SetState(PlayerController.PlayerStates.Wall);
            return;
        }
    }

    public override void Exit() {}
}
