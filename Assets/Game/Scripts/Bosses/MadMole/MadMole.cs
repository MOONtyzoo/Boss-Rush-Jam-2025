using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(HealthSystem))]
[RequireComponent(typeof(RoomSpinConstraint))]
public class MadMole : StateMachine<MadMole>
{
    public enum BossStates {
        Spawning, Idle, Jump, Stomp, Roar, SuperJump, Dead,
    }

    // Assign each state to an enum so that it can be used with a SetState overload that takes an enum instead
    private Dictionary<BossStates, State<MadMole>> bossStates = new Dictionary<BossStates, State<MadMole>> {
        {BossStates.Spawning, new MadMoleSpawningState()},
        {BossStates.Idle, new MadMoleIdleState()},
        {BossStates.Jump, new MadMoleJumpState()},
        {BossStates.Stomp, new MadMoleStompState()},
        {BossStates.Roar, new MadMoleRoarState()},
        {BossStates.SuperJump, new MadMoleSuperJumpState()},
        {BossStates.Dead, new MadMoleDeadState()},
    };

    public Queue<BossStates> queuedStates = new Queue<BossStates>();

    public event Action OnSuperJumpSequenceStarted;
    public event Action OnSpawnStarted;
    public event Action OnJumpChargeStarted;
    public event Action OnJumped;
    public event Action OnJumpLanded;
    public event Action OnStompChargeStarted;
    public event Action OnStomped;
    public event Action OnRoarStarted;
    public event Action OnSuperJumpChargeStarted;
    public event Action OnSuperJumped;
    public event Action OnSuperFallStarted;
    public event Action OnSuperFallEnded;
    public event Action OnDamaged;
    public event Action OnDied;

    [Header("Dependencies")]
    [SerializeField] private MadMoleDataSO bossData;

    [Header("Raycasting Points")]
    [Tooltip("Origin of the floor raycast check")]
    [SerializeField] public Transform bottomPoint;

    [Header("Stage Information")]
    [Tooltip("Needs to know where the player is in order to attack it")]
    [SerializeField] private PlayerController player;
    [SerializeField] public Transform spawnPoint;
    [Tooltip("When the room is upside-down, the boss can only jump on the platforms")]
    [SerializeField] public List<Transform> upsideDownPlatformPoints;
    [Tooltip("The point furthest left that the boss can jump to")]
    [SerializeField] private Transform jumpRangeLeftPoint;
    [Tooltip("The point furthest right that the boss can jump to")]
    [SerializeField] private Transform jumpRangeRightPoint;

    private new Rigidbody2D rigidbody;
    [SerializeField] private new Collider2D collider;
    private HealthSystem healthSystem;
    private RoomSpinConstraint roomSpinConstraint;
    public bool isInPhase2 = false;

    private void Awake() {
        rigidbody = GetComponent<Rigidbody2D>();
        healthSystem = GetComponent<HealthSystem>();
        roomSpinConstraint = GetComponent<RoomSpinConstraint>();
        
        InitializeStateMachine(bossStates.Values.ToList());
        rigidbody.gravityScale = bossData.gravityScale;
        healthSystem.SetMaxHealth(bossData.maxHealth);
        healthSystem.SetHealth(healthSystem.GetMaxHealth());

        healthSystem.OnDamaged += HealthSystem_OnDamaged;
    }

    private void HealthSystem_OnDamaged() {
        if (healthSystem.IsHealthDepleted()) {
            OnDied?.Invoke();
            healthSystem.SetInvulnerability(true);
            SetState(BossStates.Dead);
            
        } else {
            OnDamaged?.Invoke();
            if (healthSystem.GetHealthPercentage() <= 0.5f && !isInPhase2) {
                isInPhase2 = true;
                QueueState(BossStates.Roar);
                QueueState(BossStates.SuperJump);
            }
        }
    }

    // This is handling all of the behaviors and inputs present throughout all states
    private new void Update() {
        base.Update();
    }

    public void MoveX(float targetVelocityX, float acceleration) {
        rigidbody.linearVelocityX = Mathf.Lerp(rigidbody.linearVelocityX, targetVelocityX, acceleration * Time.deltaTime);
    }
    public Vector2 GetVelocity() => rigidbody.linearVelocity;
    public float GetVelocityX() => rigidbody.linearVelocityX;
    public float GetVelocityY() => rigidbody.linearVelocityY;
    public void SetVelocity(Vector2 velocity) => rigidbody.linearVelocity = velocity;
    public void SetVelocityX(float velocityX) => rigidbody.linearVelocityX = velocityX;
    public void SetVelocityY(float velocityY) => rigidbody.linearVelocityY = velocityY;
    public void ResetVelocity() => rigidbody.linearVelocity = Vector2.zero;
    public void SetIgnoreGravity(bool ignoreGravity) {
        rigidbody.gravityScale = ignoreGravity ? 0f : bossData.gravityScale;
    }
    public void SetGravityScale(float newGravityScale) => rigidbody.gravityScale = newGravityScale;
    public void SetCollisionsEnabled(bool newCollisionsEnabled) => collider.enabled = newCollisionsEnabled;

    public void SetKeepOrientation(bool keepOrientation) {
        roomSpinConstraint.SetKeepOrientation(keepOrientation);
    }

    public bool IsGrounded() {
        if (collider.enabled) {
            Vector2 raycastDirection = Vector2.down;
            float raycastDistance = 0.1f;
            RaycastHit2D raycastHit = Physics2D.Raycast(bottomPoint.position, raycastDirection, raycastDistance, bossData.terrainLayerMask);
            return raycastHit;
        } else {
            return false;
        }
    }

    public bool IsFalling() => rigidbody.linearVelocityY < 0.1f;
    public bool IsDead() => healthSystem.IsHealthDepleted();
    public bool IsIdle() => GetActiveState() == bossStates[BossStates.Idle];
    public Vector2 GetPlayerPosition() => player.transform.position;
    public Vector2 GetDirectionToPlayer() => (player.transform.position - transform.position).normalized;
    public Vector2 GetDirectionToPlayerLeftRight() => (player.transform.position.x > transform.position.x) ? Vector2.right : Vector2.left;

    public MadMoleDataSO GetBossData() => bossData;

    public void InvokeOnSuperJumpSequenceStarted() => OnSuperJumpSequenceStarted?.Invoke();
    public void InvokeOnSpawnStarted() => OnSpawnStarted?.Invoke();
    public void InvokeOnJumpChargeStarted() => OnJumpChargeStarted?.Invoke();
    public void InvokeOnJumped() => OnJumped?.Invoke();
    public void InvokeOnJumpLanded() => OnJumpLanded?.Invoke();
    public void InvokeOnStompChargeStarted() => OnStompChargeStarted?.Invoke();
    public void InvokeOnStomped() => OnStomped?.Invoke();
    public void InvokeOnRoarStarted() => OnRoarStarted?.Invoke();
    public void InvokeOnSuperJumpChargeStarted() => OnSuperJumpChargeStarted?.Invoke();
    public void InvokeOnSuperJumped() => OnSuperJumped?.Invoke();
    public void InvokeOnSuperFallStarted() => OnSuperFallStarted?.Invoke();
    public void InvokeOnSuperFallEnded() => OnSuperFallEnded?.Invoke();

    public void SetState(BossStates newStateName) {
        State<MadMole> newState = bossStates[newStateName];
        SetState(newState);
    }
    public void QueueState(BossStates state) {
        queuedStates.Enqueue(state);
    }
}