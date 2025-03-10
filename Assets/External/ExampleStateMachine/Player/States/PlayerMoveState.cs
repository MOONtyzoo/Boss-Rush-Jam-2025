using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ExampleStateMachine {
public class PlayerMoveState : State<Player>
{
    public override void Enter(Player parent)
    {
        base.Enter(parent);
        RunnerObject.SetAnimation(Player.AnimationName.PlayerRun);
        RunnerObject.particles.Play();
    }

    public override void Tick(float deltaTime)
    {
        RunnerObject.FlipSpriteToFaceDirection(RunnerObject.movementInput);
    }

    public override void FixedTick(float fixedDeltaTime)
    {
        // RunnerObject.SetVelocity(RunnerObject.movementInput * RunnerObject.playerData.movementSpeed);
    }

    public override void HandleStateTransitions()
    {
        if (RunnerObject.rollInputDown)
        {
            RunnerObject.SetState(typeof(PlayerRollState));
            RunnerObject.particles.Stop();
            return;
        }
        
        if (RunnerObject.attackInputDown)
        {
            RunnerObject.SetState(typeof(PlayerAttackState));
            RunnerObject.particles.Stop();
            return;
        }

        if (RunnerObject.movementInput == Vector2.zero)
        {
            RunnerObject.SetState(typeof(PlayerIdleState));
            RunnerObject.particles.Stop();
        }
        
    }
}
}
