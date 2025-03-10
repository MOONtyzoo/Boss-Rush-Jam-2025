using System;
using UnityEngine;

/// <summary>
/// <para>The purpose of this class is to shoot out a grapple projectile, which searches for grapple points to hook onto.</para>
/// <para>It handles the logic of searching for a grapple point and displaying the visual via the LineRenderer.</para>
/// <para>The actual movement once hooked will be handled by the PlayerController, based on the communications from this class.</para>
/// </summary>
public class PlayerGrapple : MonoBehaviour
{
    public event Action OnConnected;

    [Tooltip("Prints state transitions to console")]
    [SerializeField] private bool debugMode = false;

    [SerializeField] private PrototypePlayerDataSO playerData;

    [SerializeField] private Transform hookPrefab;
    [SerializeField] private LineRenderer grappleVisualLineRenderer;
    [SerializeField] private Transform grappleSpawnPoint;

    private Transform hookTransform;
    private Transform connectedGrapplePoint;
    private float hookRadius = 0.1f;

    private enum GrappleState {
        Deactive,
        Shooting,
        Retracting,
        Connected
    }
    private GrappleState currentState = GrappleState.Deactive;

    private void Awake() {
        Deactivate();
    }
    
    public void Shoot(Vector2 targetPosition) {
        Vector2 hookPosition = grappleSpawnPoint.position;
        Vector2 travelDirection = (targetPosition - hookPosition).normalized;

        hookTransform = Instantiate(hookPrefab, null);
        hookTransform.position = hookPosition;
        hookTransform.up = travelDirection;

        ChangeState(GrappleState.Shooting);
    }

    public void Disconnect() {
        if (currentState == GrappleState.Connected) {
            ChangeState(GrappleState.Retracting);
        }
    }

    public void Deactivate() {
        ChangeState(GrappleState.Deactive);
        if (hookTransform != null) Destroy(hookTransform.gameObject);
    }

    private void Update() {
        if (IsActive()) {
            UpdateGrappleVisual();
        }
    }

    private void UpdateGrappleVisual() {
        Vector3 lineStartPosition = grappleSpawnPoint.position;
        Vector3 lineEndPosition = GetHookPosition();

        Vector3[] linePositions = { lineStartPosition, lineEndPosition };
        grappleVisualLineRenderer.SetPositions(linePositions);
    }

    private void FixedUpdate() {
        switch(currentState) {
            case GrappleState.Shooting:
                MoveHookForward();
                if (TryGetGrapplePointInRange(out Transform grapplePoint)) {
                    ConnectToGrapplePoint(grapplePoint);
                    ChangeState(GrappleState.Connected);
                    break;
                }
                if (GetHookDistance() > playerData.grappleRange) { ChangeState(GrappleState.Retracting); }
                if (IsTouchingObstacle()) { ChangeState(GrappleState.Retracting); }
                break;

            case GrappleState.Retracting:
                MoveHookBackward();
                if (GetHookDistance() < 0.25f) { Deactivate(); }
                break;

            case GrappleState.Connected:
                break;
        }
    }

    private void MoveHookForward() {
        Vector2 newPosition = GetHookPosition() + playerData.grappleShootSpeed * GetHookDirection() * Time.deltaTime;
        SetHookPosition(newPosition);
    }

    private void MoveHookBackward() {
        Vector2 directionToSpawnPoint = ((Vector2)grappleSpawnPoint.transform.position - GetHookPosition()).normalized;
        Vector2 newPosition = GetHookPosition() + playerData.grappleRetractSpeed * directionToSpawnPoint * Time.deltaTime; 
        SetHookPosition(newPosition);
    }

    private void ConnectToGrapplePoint(Transform grapplePoint) {
        connectedGrapplePoint = grapplePoint;
        SetHookPosition(grapplePoint.position);
        OnConnected?.Invoke();
    }

    private void ChangeState(GrappleState newState) {
        if (debugMode) Debug.Log($"Grapple: Transition from {currentState} to {newState}!");
        currentState = newState;

        if (newState == GrappleState.Deactive) {
            grappleVisualLineRenderer.gameObject.SetActive(false);
        } else {
            UpdateGrappleVisual();
            grappleVisualLineRenderer.gameObject.SetActive(true);
        }
    }

    private bool IsTouchingObstacle() {
        RaycastHit2D hit = Physics2D.CircleCast(GetHookPosition(), hookRadius, Vector2.up, 0f, playerData.grappleObstacleMask);
        return hit;
    }

    private bool TryGetGrapplePointInRange(out Transform grapplePoint) {
        RaycastHit2D hit = Physics2D.CircleCast(GetHookPosition(), hookRadius, Vector2.up, 0f, playerData.grapplePointLayerMask);
        if (hit) {
            grapplePoint = hit.collider.gameObject.transform;
            return true;
        } else {
            grapplePoint = null;
            return false;
        }
    }

    private float GetHookDistance() => Vector2.Distance(GetHookPosition(), grappleSpawnPoint.transform.position);
    private Vector2 GetHookPosition() => hookTransform.position;
    private void SetHookPosition(Vector2 newPosition) => hookTransform.position = newPosition;
    private Vector2 GetHookDirection() => hookTransform.up;

    public bool IsActive() => currentState != GrappleState.Deactive;
    public bool IsConnected() => currentState == GrappleState.Connected;
    public bool CanShoot() => !IsActive();

    public Transform GetConnectedGrapplePoint() => connectedGrapplePoint;
}
