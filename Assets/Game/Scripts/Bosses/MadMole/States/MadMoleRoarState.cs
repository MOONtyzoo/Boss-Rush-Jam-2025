using System.Collections;
using DG.Tweening;
using UnityEngine;

public class MadMoleRoarState : State<MadMole>
{
    private MadMoleDataSO bossData;

    private float roarTimer = 0;

    protected override void Enter() {
        bossData = RunnerObject.GetBossData();

        roarTimer = 0;
        RunnerObject.InvokeOnRoarStarted();
    }

    public override void FixedTick(float fixedDeltaTime) {}
    public override void Tick(float deltaTime) {
        roarTimer += Time.deltaTime;
    }
    public override void HandleStateTransitions() {
        if (roarTimer >= bossData.roarDuration) {
            RunnerObject.SetState(MadMole.BossStates.Idle);
        }
    }

    public override void Exit() {}
}
