using UnityEngine;

public class MadMoleIdleState : State<MadMole>
{
    private MadMoleDataSO bossData;

    private float decisionTimer;

    protected override void Enter() {
        bossData = RunnerObject.GetBossData();

        decisionTimer = 0;
    }

    public override void FixedTick(float fixedDeltaTime) {}

    public override void Tick(float deltaTime) {
        decisionTimer += Time.deltaTime;
    }

    public override void HandleStateTransitions() {
        if (RunnerObject.queuedStates.Count > 0) {
            MadMole.BossStates nextState = RunnerObject.queuedStates.Dequeue();
            if (nextState == MadMole.BossStates.Roar && RunnerObject.isInPhase2) {
                RunnerObject.InvokeOnSuperJumpSequenceStarted();
            }
            RunnerObject.SetState(nextState);
        } else if (decisionTimer > bossData.decisionTime) {
            DecideNextState();
        }
    }

    public void DecideNextState() {
        float randomNum = Random.Range(1, 101);
        if (randomNum < 75) {
            RunnerObject.SetState(MadMole.BossStates.Jump);
        } else {
            RunnerObject.SetState(MadMole.BossStates.Stomp);
        }
    }

    public void DebugDecideNextState() {
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            RunnerObject.SetState(MadMole.BossStates.Jump);
        } else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            RunnerObject.SetState(MadMole.BossStates.Stomp);
        } else if (Input.GetKeyDown(KeyCode.Alpha3)) {
            RunnerObject.SetState(MadMole.BossStates.Roar);
        } else if (Input.GetKeyDown(KeyCode.Alpha4)) {
            RunnerObject.SetState(MadMole.BossStates.SuperJump);
        }
    }

    public override void Exit() {}
}
