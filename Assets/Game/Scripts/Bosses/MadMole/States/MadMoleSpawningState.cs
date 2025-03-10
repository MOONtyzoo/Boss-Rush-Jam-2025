using System.Collections;
using UnityEngine;

public class MadMoleSpawningState : State<MadMole>
{
    private MadMoleDataSO bossData;

    protected override void Enter() {
        RunnerObject.transform.position = RunnerObject.spawnPoint.position;
        RunnerObject.SetCollisionsEnabled(false);
        RunnerObject.InvokeOnSpawnStarted();
    }

    public override void FixedTick(float fixedDeltaTime) {
        if (RunnerObject.transform.position.y < 2f) {
            RunnerObject.SetCollisionsEnabled(true);
        }
    }

    public override void Tick(float deltaTime) {}
    
    public override void HandleStateTransitions() {
        if (RunnerObject.IsGrounded() && RunnerObject.transform.position.y < 2f) {
            RunnerObject.InvokeOnJumpLanded();
            RunnerObject.SetState(MadMole.BossStates.Idle);
        }
    }

    public override void Exit() {
        RunnerObject.QueueState(MadMole.BossStates.Roar);
    }
}
