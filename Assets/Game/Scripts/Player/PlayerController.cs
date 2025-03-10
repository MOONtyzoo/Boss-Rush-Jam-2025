using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(HealthSystem))]
[RequireComponent(typeof(RoomSpinConstraint))]
public class PlayerController : StateMachine<PlayerController>
{
    public enum PlayerStates {
        Grounded, Aerial, Wall,
        Grappling, StickingToPoint,
        Hurt, Dead,
    }

    // Assign each state to an enum so that it can be used with a SetState overload that takes an enum instead
    private Dictionary<PlayerStates, State<PlayerController>> playerStates = new Dictionary<PlayerStates, State<PlayerController>> {
        {PlayerStates.Grounded, new PlayerGroundedState()}, 
        {PlayerStates.Aerial, new PlayerAerialState()},
        {PlayerStates.Wall, new PlayerWallState()}, 
        {PlayerStates.Grappling, new PlayerGrapplingState()}, 
        {PlayerStates.StickingToPoint, new PlayerStickingToPointState()}, 
        {PlayerStates.Hurt, new PlayerHurtState()},
        {PlayerStates.Dead, new PlayerDeadState()}
    };

    public event Action<float> OnMoved;
    public event Action OnShotGun;
    public event Action OnShotGrapple;
    public event Action OnJumped;
    public event Action OnWallJumped;
    public event Action OnCrouchStarted;
    public event Action OnCrouchEnded;
    public event Action OnWallGrabStarted;
    public event Action OnWallGrabEnded;
    public event Action OnGrappleStarted;
    public event Action OnGrappleCanceled;
    public event Action OnGrappleEnded;
    public event Action OnStickStarted;
    public event Action OnStickEnded;
    public event Action OnHurtStarted;
    public event Action OnHurtEnded;
    public event Action OnDied;

    [Header("Dependencies")]
    [SerializeField] private PrototypePlayerDataSO playerData;
    [SerializeField] private InputReaderSO inputReader;
    [SerializeField] private PlayerWebShooter webShooter;
    [SerializeField] private PlayerGrapple grapple;
    [SerializeField] private new Collider2D collider;
    private new Rigidbody2D rigidbody;
    private HealthSystem healthSystem;
    private RoomSpinConstraint roomSpinConstraint;

    [Header("Raycasting Points")]
    [Tooltip("Origin of the left wall raycast check")]
    [SerializeField] private Transform leftPoint;
    [Tooltip("Origin of the right wall raycast check")]
    [SerializeField] private Transform rightPoint;
    [Tooltip("Origin of the floor raycast check")]
    [SerializeField] private Transform bottomPoint;


    private Vector2 aimPosition;

    int jumpsRemaining = 0;
    int wallJumpsRemaining = 0;

    private void Awake() {
        rigidbody = GetComponent<Rigidbody2D>();
        healthSystem = GetComponent<HealthSystem>();
        roomSpinConstraint = GetComponent<RoomSpinConstraint>();
        
        InitializeStateMachine(playerStates.Values.ToList());
        healthSystem.SetMaxHealth(playerData.maxHealth);
        healthSystem.SetHealth(healthSystem.GetMaxHealth());

        grapple.OnConnected += Grapple_OnConnected;
        healthSystem.OnDamaged += HealthSystem_OnDamaged;
    }

    // This is handling all of the behaviors and inputs present throughout all states
    private new void Update() {
        base.Update();

        aimPosition = inputReader.GetInputAimPosition(transform);
        if (inputReader.GetInputShootDown() && webShooter.CanShoot()) {
            ShootGun();
        }

        if (inputReader.GetInputGrappleDown() && grapple.CanShoot()) {
            ShootGrapple();
        }
    }

    private void Grapple_OnConnected() {
        SetState(PlayerStates.Grappling);
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (playerData.damagingMask.Contains(other.gameObject.layer)) {
            healthSystem?.Damage(1);
        }
    }

    private void HealthSystem_OnDamaged() {
        if (!healthSystem.IsHealthDepleted()) {
            SetState(PlayerStates.Hurt);
            if (grapple.IsActive()) grapple.Deactivate(); 
            StartCoroutine(InvulnerabilityCoroutine());
        } else {
            OnDied?.Invoke();
            SetState(PlayerStates.Dead);
            if (grapple.IsActive()) grapple.Deactivate();
            healthSystem.SetInvulnerability(true);
        }
    }

    private IEnumerator InvulnerabilityCoroutine() {
        healthSystem.SetInvulnerability(true);

        float invulnerabilityTimer = 0;
        while(invulnerabilityTimer < playerData.invulnerabilityDuration) {
            invulnerabilityTimer += Time.deltaTime;
            yield return null;
        }

        healthSystem.SetInvulnerability(false);
    }

    public void MoveX(float targetVelocityX, float acceleration) {
        rigidbody.linearVelocityX = Mathf.Lerp(rigidbody.linearVelocityX, targetVelocityX, acceleration * Time.deltaTime);
        OnMoved?.Invoke(targetVelocityX/Mathf.Abs(targetVelocityX));
    }
    public Vector2 GetVelocity() => rigidbody.linearVelocity;
    public float GetVelocityX() => rigidbody.linearVelocityX;
    public float GetVelocityY() => rigidbody.linearVelocityY;
    public void SetVelocity(Vector2 velocity) => rigidbody.linearVelocity = velocity;
    public void SetVelocityX(float velocityX) => rigidbody.linearVelocityX = velocityX;
    public void SetVelocityY(float velocityY) => rigidbody.linearVelocityY = velocityY;
    public void ResetVelocity() => rigidbody.linearVelocity = Vector2.zero;
    public void SetCollisionsEnabled(bool newCollisionsEnabled) => collider.enabled = newCollisionsEnabled;

    public bool CanJump() => IsGrounded() && jumpsRemaining > 0;
    public void RefreshJumps()  { jumpsRemaining = 1; wallJumpsRemaining = 1; }
    public void Jump() {
        rigidbody.linearVelocityY = playerData.jumpSpeed;
        jumpsRemaining--;
        OnJumped?.Invoke();
    }

    public bool CanWallJump() => IsTouchingWall() && wallJumpsRemaining > 0;
    public void RefreshWallJumps() => wallJumpsRemaining = 1;
    public void WallJump() {
        // Does the same thing physically as jump, but could be changed to add extra horizontal velocity or something
        rigidbody.linearVelocityY = playerData.jumpSpeed;
        wallJumpsRemaining--;
        OnWallJumped?.Invoke();
    }

    public void ShootGun() {
        if (playerData.shootAimAssister?.TryGetTargetPositionInDirection(transform.position, aimPosition, out Vector2 targetPosition) ?? false) {
            webShooter.Shoot(targetPosition);
        } else {
            webShooter.Shoot(aimPosition);
        }
        OnShotGun?.Invoke();
    }

    public void ShootGrapple() {
        if (grapple.CanShoot()) {
            if (playerData.grappleAimAssister?.TryGetTargetPositionInDirection(transform.position, aimPosition, out Vector2 targetPosition) ?? false) {
                grapple.Shoot(targetPosition);
            } else {
                grapple.Shoot(aimPosition);
            }
            OnShotGrapple?.Invoke();
        }
    }
    public void SnapToGrapplePoint() => transform.position = grapple.GetConnectedGrapplePoint().position;

    public void SetIgnoreGravity(bool ignoreGravity) {
        rigidbody.gravityScale = ignoreGravity ? 0f : playerData.gravityScale;
    }

    public void SetKeepOrientation(bool keepOrientation) {
        roomSpinConstraint.SetKeepOrientation(keepOrientation);
    }

    public bool IsGrounded() {
        Vector2 raycastDirection = Vector2.down;
        float raycastDistance = 0.1f;
        RaycastHit2D raycastHit = Physics2D.Raycast(bottomPoint.position, raycastDirection, raycastDistance, playerData.terrainLayerMask);
        return raycastHit;
    }

    public bool IsTouchingWall() {
        return IsTouchingLeftWall() || IsTouchingRightWall();
    }

    public bool IsTouchingLeftWall() {
        Vector2 raycastDirection = Vector2.left;
        float raycastDistance = 0.1f;
        RaycastHit2D leftRaycastHit = Physics2D.Raycast(leftPoint.position, raycastDirection, raycastDistance, playerData.terrainLayerMask);
        return leftRaycastHit;
    }

    public bool IsTouchingRightWall() {
        Vector2 raycastDirection = Vector2.right;
        float raycastDistance = 0.1f;
        RaycastHit2D rightRaycastHit = Physics2D.Raycast(rightPoint.position, raycastDirection, raycastDistance, playerData.terrainLayerMask);
        return rightRaycastHit;
    }


    public bool IsPushingAgainstWall() {
        return (IsTouchingLeftWall() && inputReader.GetInputMoveX() < 0)
                || (IsTouchingRightWall() && inputReader.GetInputMoveX() > 0);
    }

    public bool IsMoving() => Mathf.Abs(inputReader.GetInputMoveX()) >= 0.1f;
    public bool IsFalling() => rigidbody.linearVelocityY < 0.1f;
    public bool IsDead() => healthSystem.IsHealthDepleted();
    public bool IsGrappling() => GetActiveState() == playerStates[PlayerStates.Grappling];

    public Vector2 GetAimPosition() => aimPosition;

    public PrototypePlayerDataSO GetPlayerData() => playerData;
    public InputReaderSO GetInputReader() => inputReader;
    public PlayerGrapple GetGrapple() => grapple;

    public void InvokeOnCrouchStarted() => OnCrouchStarted?.Invoke();
    public void InvokeOnCrouchEnded() => OnCrouchEnded?.Invoke();
    public void InvokeOnWallGrabStarted() => OnWallGrabStarted?.Invoke();
    public void InvokeOnWallGrabEnded() => OnWallGrabEnded?.Invoke();
    public void InvokeOnGrappleStarted() => OnGrappleStarted?.Invoke();
    public void InvokeOnGrappleCanceled() => OnGrappleCanceled?.Invoke();
    public void InvokeOnGrappleEnded() => OnGrappleEnded?.Invoke();
    public void InvokeOnStickStarted() => OnStickStarted?.Invoke();
    public void InvokeOnStickEnded() => OnStickEnded?.Invoke();
    public void InvokeOnHurtStarted() => OnHurtStarted?.Invoke();
    public void InvokeOnHurtEnded() => OnHurtEnded?.Invoke();

    public void SetState(PlayerStates newStateName) {
        State<PlayerController> newState = playerStates[newStateName];
        SetState(newState);
    }
}