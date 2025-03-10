using System.Collections;
using DG.Tweening;
using UnityEngine;

public class MadMoleJumpState : State<MadMole>
{
    private MadMoleDataSO bossData;

    private Vector2 targetPosition;

    private float jumpTimer = 0;

    protected override void Enter() {
        bossData = RunnerObject.GetBossData();

        jumpTimer = 0;
        RunnerObject.StartCoroutine(JumpCoroutine());
    }

    private IEnumerator JumpCoroutine() {
        //charging started
        RunnerObject.InvokeOnJumpChargeStarted();
        while (jumpTimer < bossData.jumpChargeTime) {
            yield return null;
        }

        // changing ended, start jump
        targetPosition = CalculateJumpTargetPosition();
        float velocityX = (targetPosition.x - RunnerObject.transform.position.x)/(bossData.jumpLandTime - bossData.jumpChargeTime);
        RunnerObject.SetVelocityX(velocityX);
        RunnerObject.SetVelocityY(bossData.jumpStrength);
        RunnerObject.SetGravityScale(bossData.jumpUpwardGravityScale);
        RunnerObject.InvokeOnJumped();

        while (jumpTimer < bossData.jumpPeakTime) {
            yield return null;
        }

        // jump midpoint reached, start falling
        RunnerObject.SetGravityScale(bossData.jumpFallGravityScale);

        while (jumpTimer < bossData.jumpLandTime && !RunnerObject.IsGrounded()) {
            yield return null;
        }

        // Landed on floor
        RunnerObject.InvokeOnJumpLanded();
        RunnerObject.SetState(MadMole.BossStates.Idle);
    }

    public Vector2 CalculateJumpTargetPosition() {
        Vector2 jumpTargetPosition = Vector2.zero;

        if (RoomSpinner.Instance.GetOrientation() == Orientation.RightSideUp) {
            float distanceToPlayer = RunnerObject.GetPlayerPosition().x - RunnerObject.transform.position.x;
            jumpTargetPosition.x = RunnerObject.transform.position.x + Mathf.Clamp(distanceToPlayer, -bossData.jumpDistanceMax, bossData.jumpDistanceMax);
            jumpTargetPosition.y = RunnerObject.transform.position.y;
        } else if (RoomSpinner.Instance.GetOrientation() == Orientation.UpsideDown) {
            jumpTargetPosition = RunnerObject.upsideDownPlatformPoints[Random.Range(0, RunnerObject.upsideDownPlatformPoints.Count)].position;
        }

        return jumpTargetPosition;
    }

    public override void FixedTick(float fixedDeltaTime) {}
    public override void Tick(float deltaTime) {
        jumpTimer += Time.deltaTime;
    }
    public override void HandleStateTransitions() {}

    public override void Exit() {
        RunnerObject.SetVelocityX(0f);
        RunnerObject.SetIgnoreGravity(false);
    }
}
