using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ExampleStateMachine {
public class PlayerAttackState : State<Player>
{
    private enum AttackType {
        Attack1,
        Attack2,
        Attack3
    }

    private AttackType currentAttack;
    private float currentAttackElapsedTime;
    private float currentAttackDuration;
    private float currentAttackMovementPush;
    private int currentAttackDamage;
    private float currentAttackHitboxSpawnTime;
    private Vector2 attackDirection;
    private bool hitboxSpawned;

    private bool hasPlayerInputAttack;

    public override void Enter(Player parent)
    {
        base.Enter(parent);
        RunnerObject.SetVelocity(Vector2.zero);
        SetCurrentAttack(AttackType.Attack1);
    }

    public override void Tick(float deltaTime)
    {
        currentAttackElapsedTime += deltaTime;

        // If you click the attack button again before the attack ends, then the next attack will play
        if (RunnerObject.attackInputDown) {
            hasPlayerInputAttack = true;
        }

        if (currentAttackElapsedTime >= currentAttackDuration)
        {
            if (hasPlayerInputAttack && HasNextAttack()) {
                GoToNextAttack();
            } else {
                RunnerObject.SetAnimation(Player.AnimationName.PlayerReturnToIdle);
            }
        }
    }
    

    public override void FixedTick(float fixedDeltaTime)
    {
        Vector2 attackPushVelocity = Mathf.Lerp(currentAttackMovementPush, 0, GetAttackProgressNormalized()) * attackDirection;
        // Vector2 moveVelocity = 0.3f * RunnerObject.playerData.movementSpeed * RunnerObject.movementInput;;

        // attackPushVelocity is what pushes the player in the aim direction
        // Vector2 newVelocity = attackPushVelocity + moveVelocity;

        // RunnerObject.SetVelocity(newVelocity);

        if (currentAttackElapsedTime > currentAttackHitboxSpawnTime && !hitboxSpawned) {
            hitboxSpawned = true;
            // RunnerObject.hitbox.CreateHitBoxPrefab(currentAttackDamage, true);
            //SoundManager.Instance.PlaySound(GameSoundsData.Sound.Swipe, RunnerObject.transform.position);
        }
    }

    public override void HandleStateTransitions()
    {
        if (currentAttackElapsedTime >= currentAttackDuration + 0.1f)
        {
            RunnerObject.SetState(typeof(PlayerIdleState));
        }
    }
    
    private void SetCurrentAttack(AttackType newAttack) {
        currentAttackElapsedTime = 0;
        hasPlayerInputAttack = false;
        hitboxSpawned = false;

        attackDirection = RunnerObject.lookInput;
        RunnerObject.FlipSpriteToFaceDirection(RunnerObject.lookInput);

        switch(newAttack) {
            case AttackType.Attack1:
                // currentAttackDuration = RunnerObject.playerData.attack1Duration;
                // currentAttackMovementPush = RunnerObject.playerData.attack1MovementPush;
                // currentAttackHitboxSpawnTime = RunnerObject.playerData.attack1HitboxSpawnTime;
                // currentAttackDamage = RunnerObject.playerData.attack1Damage;
                RunnerObject.SetAnimation(Player.AnimationName.PlayerAttack1);
                break;
            case AttackType.Attack2:
                // currentAttackDuration = RunnerObject.playerData.attack2Duration;
                // currentAttackMovementPush = RunnerObject.playerData.attack2MovementPush;
                // currentAttackHitboxSpawnTime = RunnerObject.playerData.attack2HitboxSpawnTime;
                // currentAttackDamage = RunnerObject.playerData.attack2Damage;
                RunnerObject.SetAnimation(Player.AnimationName.PlayerAttack2);
                break;
            case AttackType.Attack3:
                // currentAttackDuration = RunnerObject.playerData.attack3Duration;
                // currentAttackMovementPush = RunnerObject.playerData.attack3MovementPush;
                // currentAttackHitboxSpawnTime = RunnerObject.playerData.attack3HitboxSpawnTime;
                // currentAttackDamage = RunnerObject.playerData.attack3Damage;
                RunnerObject.SetAnimation(Player.AnimationName.PlayerAttack3);
                break;
        }

        currentAttack = newAttack;
    }

    private bool HasNextAttack() {
        return (int)currentAttack < 2;
    }

    private void GoToNextAttack() {
        SetCurrentAttack(currentAttack + 1);
    }
    
    private float GetAttackProgressNormalized() {
        return currentAttackElapsedTime/currentAttackDuration;
    }
}
}