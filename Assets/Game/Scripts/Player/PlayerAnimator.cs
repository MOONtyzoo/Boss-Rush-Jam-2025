using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [Tooltip("This script listens to events in the player to choose animations")]
    [SerializeField] private PlayerController player;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private Animator animator;

    [Tooltip("The script will rotate this transform to point at the mouse")]
    [SerializeField] private Transform aimableArm;
    [Tooltip("The position of the arm when player is facing left")]
    [SerializeField] private Transform armLeftPoint;
    [Tooltip("The position of the arm when player is facing right")]
    [SerializeField] private Transform armRightPoint;

    [Tooltip("Affects how quickly the rotates toward the cursor")]
    [SerializeField] private float armRotationSpeed;

    private Vector2 armAimDirection;

    private const string ANIMATOR_BOOL_ISMOVING = "IsMoving";
    private const string ANIMATOR_BOOL_ISGROUNDED = "IsGrounded";
    private const string ANIMATOR_BOOL_ISFALLING = "IsFalling";
    private const string ANIMATOR_BOOL_ISGRABBINGWALL = "IsGrabbingWall";
    private const string ANIMATOR_BOOL_ISCROUCHING = "IsCrouching";
    private const string ANIMATOR_BOOL_ISSTICKING = "IsSticking";
    private const string ANIMATOR_BOOL_ISHURTING = "IsHurt";

    private const string ANIMATOR_TRIGGER_JUMP = "Jump";
    private const string ANIMATOR_TRIGGER_GRAPPLECONNECTED = "GrappleConnected";
    private const string ANIMATOR_TRIGGER_GRAPPLECANCELED = "GrappleCanceled";

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Start() {
        player.OnMoved += (float moveInput) => FlipSpriteToFaceMoveInput(moveInput);
        player.OnJumped += () => animator.SetTrigger(ANIMATOR_TRIGGER_JUMP);
        player.OnWallJumped += () => animator.SetTrigger(ANIMATOR_TRIGGER_JUMP);
        player.OnCrouchStarted += () => animator.SetBool(ANIMATOR_BOOL_ISCROUCHING, true);
        player.OnCrouchEnded += () => animator.SetBool(ANIMATOR_BOOL_ISCROUCHING, false);
        player.OnStickStarted += () => animator.SetBool(ANIMATOR_BOOL_ISSTICKING, true);
        player.OnStickEnded += () => animator.SetBool(ANIMATOR_BOOL_ISSTICKING, false);
        player.OnHurtStarted += () => animator.SetBool(ANIMATOR_BOOL_ISHURTING, true);
        player.OnHurtEnded += () => animator.SetBool(ANIMATOR_BOOL_ISHURTING, false);
        player.OnGrappleStarted += () => animator.SetTrigger(ANIMATOR_TRIGGER_GRAPPLECONNECTED);
        player.OnGrappleCanceled += () => animator.SetTrigger(ANIMATOR_TRIGGER_GRAPPLECANCELED);
    }

    private void Update() {
        AnimateAimableArm();
        animator.SetBool(ANIMATOR_BOOL_ISMOVING, player.IsMoving());
        animator.SetBool(ANIMATOR_BOOL_ISGROUNDED, player.IsGrounded());
        animator.SetBool(ANIMATOR_BOOL_ISFALLING, player.IsFalling());
        animator.SetBool(ANIMATOR_BOOL_ISGRABBINGWALL, player.IsPushingAgainstWall());
    }

    private void FlipSpriteToFaceMoveInput(float moveInput) {
        if (moveInput > 0) {
            spriteRenderer.flipX = false;
        } else if (moveInput < 0) {
            spriteRenderer.flipX = true;
        }
    }

    private void AnimateAimableArm() {
        aimableArm.transform.position = IsMouseRightOfPlayer() ? armRightPoint.transform.position : armLeftPoint.transform.position;

        Vector2 aimPosition = player.GetAimPosition();
        Vector2 armPivotPosition = aimableArm.position;

        Vector2 targetAimDirection =  (aimPosition - armPivotPosition).normalized;
        armAimDirection = Vector3.Slerp(armAimDirection, targetAimDirection, armRotationSpeed * Time.deltaTime);
        
        aimableArm.transform.up = armAimDirection;
    }

    private bool IsMouseRightOfPlayer() {
        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return mouseWorldPosition.x >= transform.position.x;
    }
}
