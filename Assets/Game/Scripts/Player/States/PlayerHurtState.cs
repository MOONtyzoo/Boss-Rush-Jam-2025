using UnityEngine;

public class PlayerHurtState : State<PlayerController>
{
    private PrototypePlayerDataSO playerData;

    private float hurtTimer = 0;

    protected override void Enter()
    {
        playerData = RunnerObject.GetPlayerData();

        RunnerObject.SetIgnoreGravity(true);
        hurtTimer = 0;

        RunnerObject.InvokeOnHurtStarted();
        RunnerObject.ResetVelocity();
    }

    public override void FixedTick(float fixedDeltaTime) {}

    public override void Tick(float deltaTime) {
        hurtTimer += deltaTime;
    }

    public override void HandleStateTransitions() {
        if (hurtTimer > playerData.hurtStunDuration) {
            RunnerObject.SetState(PlayerController.PlayerStates.Aerial);
        }
    }

    public override void Exit() {
        RunnerObject.SetIgnoreGravity(false);

        RunnerObject.InvokeOnHurtEnded();
    }
}
