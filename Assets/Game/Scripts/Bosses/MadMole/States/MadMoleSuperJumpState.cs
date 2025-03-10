using System.Collections;
using DG.Tweening;
using UnityEngine;

public class MadMoleSuperJumpState : State<MadMole>
{
    private MadMoleDataSO bossData;

    private Vector2 targetPosition;

    private float superJumpTimer = 0;

    protected override void Enter() {
        bossData = RunnerObject.GetBossData();

        superJumpTimer = 0;
        RunnerObject.ResetVelocity();
        RunnerObject.SetGravityScale(0);
        RunnerObject.StartCoroutine(SuperJumpCoroutine());
    }

    private IEnumerator SuperJumpCoroutine() {
        //charging started
        RunnerObject.InvokeOnSuperJumpChargeStarted();
        while (superJumpTimer < bossData.superJumpChargeTime) {
            yield return null;
        }

        // changing ended, start jump
        Sequence jumpSequence = DOTween.Sequence();
        float jumpSequenceDuration = bossData.superJumpPeakTime - bossData.superJumpChargeTime;
        float jumpTargetXPosition = CalculateSuperJumpTargetXPosition();
        float jumpTargetYPosition = RunnerObject.transform.position.y;
        float jumpPeakYPosition = RunnerObject.transform.position.y + bossData.superJumpHeight;
        jumpSequence.Append(RunnerObject.transform.DOMoveX(jumpTargetXPosition, jumpSequenceDuration).SetEase(Ease.OutQuad));
        jumpSequence.Join(RunnerObject.transform.DOMoveY(jumpPeakYPosition, jumpSequenceDuration).SetEase(Ease.OutQuart));
        RunnerObject.InvokeOnSuperJumped();

        while (jumpSequence.active) {
            yield return null;
        }

        // Arrived at peak of jump, now pause for a little bit

        while (superJumpTimer < bossData.superJumpFallTime) {
            yield return null;
        }

        // jump midpoint reached, start falling
        float fallTweenDuration = bossData.superJumpLandTime - bossData.superJumpFallTime;
        Tween fallTween = RunnerObject.transform.DOMoveY(jumpTargetYPosition, fallTweenDuration).SetEase(Ease.Linear);
        RunnerObject.InvokeOnSuperFallStarted();

        while (fallTween.active) {
            yield return null;
        }

        // Landed on floor
        RoomSpinner.SpinDirection roomSpinDirection = (RunnerObject.transform.position.x > 0) ? RoomSpinner.SpinDirection.Clockwise180 : RoomSpinner.SpinDirection.CounterClockwise180;
        RoomSpinner.Instance.SpinRoom(roomSpinDirection);
        RunnerObject.ResetVelocity();
        RunnerObject.InvokeOnSuperFallEnded();
        RunnerObject.SetState(MadMole.BossStates.Idle);
    }

    public float CalculateSuperJumpTargetXPosition() {
        Vector2 jumpTargetPosition = Vector2.zero;

        return RunnerObject.upsideDownPlatformPoints[Random.Range(0, RunnerObject.upsideDownPlatformPoints.Count)].position.x;
    }

    public override void FixedTick(float fixedDeltaTime) {}
    public override void Tick(float deltaTime) {
        superJumpTimer += Time.deltaTime;
    }
    public override void HandleStateTransitions() {}

    public override void Exit() {
        RunnerObject.SetIgnoreGravity(false);
    }
}
