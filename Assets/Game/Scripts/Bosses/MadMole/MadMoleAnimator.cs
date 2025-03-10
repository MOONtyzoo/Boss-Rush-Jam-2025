using UnityEngine;

public class MadMoleAnimator : MonoBehaviour
{
    [Tooltip("This script listens to events in the boss to choose animations")]
    [SerializeField] private MadMole boss;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private Animator animator;

    private const string ANIMATOR_TRIGGER_JUMPCHARGEUP = "JumpChargeUp";
    private const string ANIMATOR_TRIGGER_JUMP = "Jump";
    private const string ANIMATOR_TRIGGER_STOMPCHARGEUP = "StompChargeUp";
    private const string ANIMATOR_TRIGGER_STOMP = "Stomp";
    private const string ANIMATOR_TRIGGER_ROAR = "Roar";
    private const string ANIMATOR_TRIGGER_SUPERJUMPCHARGEUP = "SuperJumpChargeUp";
    private const string ANIMATOR_TRIGGER_SUPERJUMP = "SuperJump";
    private const string ANIMATOR_TRIGGER_SUPERFALL = "SuperFall";
    private const string ANIMATOR_TRIGGER_SUPERFALLENDED = "SuperFallEnded";

    private const string ANIMATOR_BOOL_ISFALLING = "IsFalling";
    private const string ANIMATOR_BOOL_ISGROUNDED = "IsGrounded";
    private const string ANIMATOR_BOOL_ISIDLE = "IsIdle";
    private const string ANIMATOR_BOOL_ISROOMSPINNING = "IsRoomSpinning";

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Start() {
        boss.OnSpawnStarted += () => animator.Play("Fall");
        boss.OnJumpChargeStarted += () => animator.SetTrigger(ANIMATOR_TRIGGER_JUMPCHARGEUP);
        boss.OnJumped += () => animator.SetTrigger(ANIMATOR_TRIGGER_JUMP);
        boss.OnStompChargeStarted += () => animator.SetTrigger(ANIMATOR_TRIGGER_STOMPCHARGEUP);
        boss.OnStomped += () => animator.SetTrigger(ANIMATOR_TRIGGER_STOMP);
        boss.OnRoarStarted += () => animator.SetTrigger(ANIMATOR_TRIGGER_ROAR);
        boss.OnSuperJumpChargeStarted += () => animator.SetTrigger(ANIMATOR_TRIGGER_SUPERJUMPCHARGEUP);
        boss.OnSuperJumped += () => animator.SetTrigger(ANIMATOR_TRIGGER_SUPERJUMP);
        boss.OnSuperFallStarted += () => animator.SetTrigger(ANIMATOR_TRIGGER_SUPERFALL);
        boss.OnSuperFallEnded += () => animator.SetTrigger(ANIMATOR_TRIGGER_SUPERFALLENDED);
    }

    private void Update() {
        FlipSpriteToFacePlayer();
        animator.SetBool(ANIMATOR_BOOL_ISFALLING, boss.IsFalling());
        animator.SetBool(ANIMATOR_BOOL_ISGROUNDED, boss.IsGrounded());
        animator.SetBool(ANIMATOR_BOOL_ISIDLE, boss.IsIdle());
        animator.SetBool(ANIMATOR_BOOL_ISROOMSPINNING, RoomSpinner.Instance.IsSpinning());
    }

    private void FlipSpriteToFacePlayer() {
        if (boss.GetPlayerPosition().x < transform.position.x) {
            spriteRenderer.flipX = false;
        } else if (boss.GetPlayerPosition().x > transform.position.x) {
            spriteRenderer.flipX = true;
        }
    }
}
