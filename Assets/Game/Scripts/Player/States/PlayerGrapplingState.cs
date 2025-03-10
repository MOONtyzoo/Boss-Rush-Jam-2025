using UnityEngine;

public class PlayerGrapplingState : State<PlayerController>
{
    private PrototypePlayerDataSO playerData;
    private InputReaderSO inputReader;
    private PlayerGrapple grapple;

    protected override void Enter()
    {
        playerData = RunnerObject.GetPlayerData();
        inputReader = RunnerObject.GetInputReader();
        grapple = RunnerObject.GetGrapple();

        RunnerObject.SetKeepOrientation(false);
        RunnerObject.SetIgnoreGravity(true);
        RunnerObject.RefreshWallJumps();

        RunnerObject.InvokeOnGrappleStarted();
    }

    public override void FixedTick(float fixedDeltaTime)
    {
        Vector2 pullVelocity = playerData.grapplePullSpeed * GetDirectionToConnectedGrapplePoint();
        RunnerObject.SetVelocity(pullVelocity);
        RunnerObject.transform.up = GetDirectionToConnectedGrapplePoint();
    }

    public override void Tick(float deltaTime) {}

    public override void HandleStateTransitions()
    {
        if (inputReader.GetInputJumpDown()) {
            grapple.Disconnect();

            RunnerObject.SetState(PlayerController.PlayerStates.Aerial);

            RunnerObject.InvokeOnGrappleCanceled();
            return;
        }

        if (GetDistanceToConnectedGrapplePoint() < 0.25f) {
            RunnerObject.SnapToGrapplePoint();
            RunnerObject.ResetVelocity();
            grapple.Deactivate();

            RunnerObject.SetState(PlayerController.PlayerStates.StickingToPoint);

            RunnerObject.InvokeOnGrappleEnded();
            return;
        }
    }

    public override void Exit()
    {
        RunnerObject.SetKeepOrientation(true);
        RunnerObject.SetIgnoreGravity(false);
        RunnerObject.transform.up = Vector2.up;
    }

    private float GetDistanceToConnectedGrapplePoint() => Vector2.Distance((Vector2)RunnerObject.transform.position, (Vector2)grapple.GetConnectedGrapplePoint().transform.position);
    private Vector2 GetDirectionToConnectedGrapplePoint() => (grapple.GetConnectedGrapplePoint().transform.position - RunnerObject.transform.position).normalized;
}
