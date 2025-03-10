using UnityEngine;

public class PlayerStickingToPointState : State<PlayerController>
{
    private InputReaderSO inputReader;

    protected override void Enter()
    {
        inputReader = RunnerObject.GetInputReader();

        RunnerObject.SetIgnoreGravity(true);

        RunnerObject.InvokeOnStickStarted();
    }

    public override void FixedTick(float fixedDeltaTime) {}

    public override void Tick(float deltaTime) {}

    public override void HandleStateTransitions() {
        if (inputReader.GetInputJumpDown()) {
            RunnerObject.Jump();
            RunnerObject.SetState(PlayerController.PlayerStates.Aerial);
            return;
        }

        if (inputReader.GetInputCrouch()) {
            RunnerObject.SetState(PlayerController.PlayerStates.Aerial);
            return;
        }
    }

    public override void Exit() {
        RunnerObject.SetIgnoreGravity(false);

        RunnerObject.InvokeOnStickEnded();
    }
}
