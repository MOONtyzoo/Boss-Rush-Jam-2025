using System;
using UnityEngine;

/// <summary>
/// <para>The purpose of this class is to quickly prototype the basic mechanics of player movement.</para>
/// <para>Later, we will likely need to refactor this script into a new one that utilizes a proper State Machine integrated with the Input System.</para>
/// </summary>
public class PrototypePlayerController : MonoBehaviour
{
    public event Action<float> OnMoved;
    public event Action OnJumped;
    public event Action OnWallJumped;
    public event Action OnCrouched;
    public event Action OnUncrouched;
    public event Action OnShotGun;
    public event Action OnShotGrapple;
    public event Action OnGrappleConnected;
    public event Action OnGrappleCanceled;

    [Tooltip("Prints state transitions to console")]
    [SerializeField] private bool debugMode = false;

    [SerializeField] private PrototypePlayerDataSO playerData;

    [SerializeField] private PlayerWebShooter webShooter;

    [Tooltip("Layers for any kind of terrain that the movement system interacts with")]
    [SerializeField] private LayerMask terrainLayerMask;
    [SerializeField] private PlayerGrapple grapple;
    private RoomSpinConstraint roomSpinConstraint;

    [Tooltip("Origin of the left wall raycast check")]
    [SerializeField] private Transform leftPoint;
    [Tooltip("Origin of the right wall raycast check")]
    [SerializeField] private Transform rightPoint;
    [Tooltip("Origin of the floor raycast check")]
    [SerializeField] private Transform bottomPoint;

    private float inputX = 0;

    [SerializeField] private float moveSpeed;

    int jumpsRemaining = 0;
    int wallJumpsRemaining = 0;

    public enum PlayerState {
        Normal, // When the player is moving normally
        Crouching,
        Grappling, // When the player's grapple is hooked onto something and pulling them
        StickingToPoint, // When the player reaches a point and is holding on to it
    }
    private PlayerState currentState = PlayerState.Normal;

    private new Rigidbody2D rigidbody;

    private void Awake() {
        rigidbody = GetComponent<Rigidbody2D>();
        roomSpinConstraint = GetComponent<RoomSpinConstraint>();
        
        rigidbody.gravityScale = playerData.gravityScale;

        grapple.OnConnected += Grapple_OnConnected;
    }

    private void Grapple_OnConnected() {
        ChangeState(PlayerState.Grappling);
        RefreshWallJumps();
        OnGrappleConnected?.Invoke();
    }

    private void Update()
    {   
        HandleInput();
    }

    private void FixedUpdate() {
        if (IsGrounded()) {
            RefreshJumps();
        }

        if (currentState == PlayerState.Normal) {
            if (IsPushingAgainstWall() && rigidbody.linearVelocityY < 0) {
                rigidbody.linearVelocityY += playerData.wallFriction * Time.deltaTime;
                rigidbody.linearVelocityY = Mathf.Min(0, rigidbody.linearVelocityY);
            }
        }

        if (currentState == PlayerState.Crouching) {
            rigidbody.linearVelocityX = Mathf.Lerp(rigidbody.linearVelocityX, 0, 1.2f * playerData.groundedDeceleration * Time.deltaTime);
        }

        if (currentState == PlayerState.Grappling) {
            transform.up = GetDirectionToConnectedGrapplePoint();
            if (GetDistanceToConnectedGrapplePoint() < 0.25f) {
                transform.position = grapple.GetConnectedGrapplePoint().transform.position;
                grapple.Deactivate();
                rigidbody.linearVelocity = Vector2.zero;
                ChangeState(PlayerState.StickingToPoint);
            } else {
                Vector2 pullVelocity = playerData.grapplePullSpeed * GetDirectionToConnectedGrapplePoint();
                rigidbody.linearVelocity = pullVelocity;
            }
        }

        if (currentState == PlayerState.StickingToPoint) {
            rigidbody.linearVelocity = Vector2.zero;
        }
    }

    private void HandleInput() {
        switch(currentState) {
            case PlayerState.Normal:
                HandleNormalStateInput(); 
                break;
            case PlayerState.Crouching:
                HandleCrouchingStateInput();
                break;
            case PlayerState.Grappling:
                HandleGrapplingStateInput(); 
                break;
            case PlayerState.StickingToPoint: 
                HandleStickingToPointStateInput();
                break;
        }

        // This is outside of the state specific functions because the player can shoot in any state.
        if (Input.GetMouseButtonDown(0)) {
            if (webShooter.CanShoot()) {
                if (playerData.shootAimAssister?.TryGetTargetPositionInDirection(transform.position, GetMouseWorldPosition(), out Vector2 targetPosition) ?? false) {
                    webShooter.Shoot(targetPosition);
                } else {
                    webShooter.Shoot(GetMouseWorldPosition());
                }
                OnShotGun?.Invoke();
            }
        }
    }

    private void HandleNormalStateInput() {
        inputX = Input.GetAxis("Horizontal");
        if (Mathf.Abs(inputX) >= 0.01f) OnMoved?.Invoke(inputX);

        float targetVelocityX = playerData.moveSpeed * inputX;
        float acceleration = playerData.PickMoveAccelerationValue(IsGrounded(), targetVelocityX);

        rigidbody.linearVelocityX = Mathf.Lerp(rigidbody.linearVelocityX, targetVelocityX, acceleration * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space)) {
            if (CanJump()) {
                Jump();
            } else if (CanWallJump()) {
                WallJump();
            }
        }

        if (IsGrounded() && Input.GetKey(KeyCode.S)) {
            Crouch();
        }

        if (Input.GetMouseButtonDown(1)) {
            if (grapple.CanShoot()) {
                if (playerData.grappleAimAssister?.TryGetTargetPositionInDirection(transform.position, GetMouseWorldPosition(), out Vector2 targetPosition) ?? false) {
                    grapple.Shoot(targetPosition);
                } else {
                    grapple.Shoot(GetMouseWorldPosition());
                }
                OnShotGrapple?.Invoke();
            }
        }
    }

    private void HandleCrouchingStateInput() {
        if (!Input.GetKey(KeyCode.S) || !IsGrounded()) {
            Uncrouch();
        }
    }

    private void HandleGrapplingStateInput() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            grapple.Disconnect();
            ChangeState(PlayerState.Normal);
            OnGrappleCanceled?.Invoke();
        }
    }

    private void HandleStickingToPointStateInput() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            Jump();
            ChangeState(PlayerState.Normal);
        }

        if (Input.GetKey(KeyCode.S)) {
            ChangeState(PlayerState.Normal);
        }
    }

    private void Jump() {
        rigidbody.linearVelocityY = playerData.jumpSpeed;
        jumpsRemaining--;
        OnJumped?.Invoke();
    }

    private void WallJump() {
        // Does the same thing physically as jump, but could be changed to add extra horizontal velocity or something
        rigidbody.linearVelocityY = playerData.jumpSpeed;
        wallJumpsRemaining--;
        OnWallJumped?.Invoke();
    }

    private void Crouch() {
        ChangeState(PlayerState.Crouching);
        OnCrouched?.Invoke();
    }

    private void Uncrouch() {
        ChangeState(PlayerState.Normal);
        OnUncrouched?.Invoke();
    }

    private void RefreshJumps() {
        jumpsRemaining = 1;
        wallJumpsRemaining = 1;
    }

    private void RefreshWallJumps() {
        wallJumpsRemaining = 1;
    }

    private void ChangeState(PlayerState newState) {
        if (debugMode) Debug.Log($"Player: Transition from {currentState} to {newState}!");
        PlayerState oldState = currentState;
        currentState = newState;

        switch(oldState) {
            case PlayerState.Grappling:
                transform.up = Vector2.up;
                roomSpinConstraint.SetKeepOrientation(true);
                break;
        }

        switch(newState) {
            case PlayerState.Normal:
                rigidbody.gravityScale = playerData.gravityScale;
                break;
            case PlayerState.Grappling:
                rigidbody.gravityScale = 0;
                roomSpinConstraint.SetKeepOrientation(false);
                break;
            case PlayerState.StickingToPoint:
                rigidbody.gravityScale = 0;
                break;
        }
    }

    private bool CanJump() => IsGrounded() && jumpsRemaining > 0;
    private bool CanWallJump() => IsTouchingWall() && wallJumpsRemaining > 0;

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
        if (IsTouchingLeftWall() && inputX < 0) {
            return true;
        }

        if (IsTouchingRightWall() && inputX > 0) {
            return true;
        }

        return false;
    }

    public bool IsMoving() => Mathf.Abs(inputX) >= 0.1f;
    public bool IsFalling() => rigidbody.linearVelocityY < 0.1f;

    private float GetDistanceToConnectedGrapplePoint() => Vector2.Distance((Vector2)transform.position, (Vector2)grapple.GetConnectedGrapplePoint().transform.position);
    private Vector2 GetDirectionToConnectedGrapplePoint() => (grapple.GetConnectedGrapplePoint().transform.position - transform.position).normalized;

    private Vector2 GetMouseWorldPosition() => Camera.main.ScreenToWorldPoint(Input.mousePosition);
    private Vector2 GetAimDirection() => (GetMouseWorldPosition() - (Vector2)transform.position).normalized;

    public PlayerState GetCurrentState() => currentState;
}
