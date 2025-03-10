using System.Collections;
using DG.Tweening;
using UnityEngine;

public class MadMoleStompState : State<MadMole>
{
    private MadMoleDataSO bossData;

    private float stompTimer = 0;

    protected override void Enter() {
        bossData = RunnerObject.GetBossData();

        stompTimer = 0;
        RunnerObject.StartCoroutine(StompCoroutine());
    }

    private IEnumerator StompCoroutine() {
        //charging started
        RunnerObject.InvokeOnStompChargeStarted();
        while (stompTimer < bossData.stompChargeTime) {
            yield return null;
        }

        // changing ended, stomp
        ShockwaveProjectile shockwaveProjectile = GameObject.Instantiate(bossData.shockwaveProjectilePrefab);
        Vector2 projectileOffset = 1f * RunnerObject.GetDirectionToPlayerLeftRight();
        shockwaveProjectile.transform.position = RunnerObject.bottomPoint.position + (Vector3)projectileOffset;
        shockwaveProjectile.Initialize(bossData.shockwaveSpeed, RunnerObject.GetDirectionToPlayerLeftRight());
        
        RunnerObject.InvokeOnStomped();
        RunnerObject.SetState(MadMole.BossStates.Idle);
    }

    public override void FixedTick(float fixedDeltaTime) {}
    public override void Tick(float deltaTime) {
        stompTimer += Time.deltaTime;
    }
    public override void HandleStateTransitions() {}

    public override void Exit() {}
}
