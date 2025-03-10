using UnityEngine;

public class PlayerDeadState : State<PlayerController>
{
    private PrototypePlayerDataSO playerDataData;

    private float timer = 0;

    protected override void Enter() {
        RunnerObject.SetCollisionsEnabled(false);

        float randomAngle = Mathf.Deg2Rad * Random.Range(-45,45);
        Vector2 launchVelocity = new Vector2(Mathf.Sin(randomAngle), Mathf.Cos(randomAngle)) * 17;
        RunnerObject.SetVelocity(launchVelocity);
    }

    public override void FixedTick(float fixedDeltaTime) { }

    public override void Tick(float deltaTime) {
        timer += Time.deltaTime;

        RunnerObject.transform.Rotate(Vector3.forward, 480 * Time.deltaTime);

        if (timer > 3) {
            RunnerObject.ResetVelocity();
            RunnerObject.SetIgnoreGravity(true);
        }
    }
    
    public override void HandleStateTransitions() {}

    public override void Exit() { }
}
