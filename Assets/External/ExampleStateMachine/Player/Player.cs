using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ExampleStateMachine {
[RequireComponent(typeof(HealthSystem))]
public class Player : StateMachine<Player>
{
    //[SerializeField] public PlayerData playerData;
    //[SerializeField] private InputReader input;
    //[SerializeField] public AttackHitbox hitbox;
    [SerializeField] public ParticleSystem particles;

    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private new Rigidbody rigidbody;
    private SpriteFlasher spriteFlasher;
    
    public HealthSystem healthSystem { get; private set; }

    public bool IsFacingRight { get; private set; }

    private int killCount;



    // These variables propagate input to the states
    [HideInInspector] public Vector2 movementInput;
    [HideInInspector] public Vector2 lookInput;
    [HideInInspector] public bool rollInputDown; // Follows convention of Input.GetKeyDown: Only true on first frame of input
    [HideInInspector] public bool attackInputDown;

    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        healthSystem = GetComponent<HealthSystem>();
        spriteFlasher = GetComponent<SpriteFlasher>();

        List<State<Player>> playerStates = new List<State<Player>>() {
            new PlayerIdleState(),
            new PlayerMoveState(),
            new PlayerRollState(),
            new PlayerAttackState()
        };
        InitializeStateMachine(playerStates);
        //healthSystem.OnDamaged.AddListener(OnHit);
        //healthSystem.OnHealthDepleted.AddListener(PlayerDead);

        killCount = 0;
    }

    private void OnEnable()
    {
        //input.MovementEvent += HandleMove;
        //input.RollEvent += HandleRoll;
        //input.AttackEvent += HandleAttack;
        //input.LookEvent += HandleLook;
    }

    private void OnDisable()
    {
        // input.MovementEvent -= HandleMove;
        // input.RollEvent -= HandleRoll;
        // input.AttackEvent -= HandleAttack;
        // input.LookEvent -= HandleLook;
    }

    protected override void Update()
    {
        base.Update();
        rollInputDown = false;
        attackInputDown = false;
    }

    private void OnHit() {
        // SoundManager.Instance.PlaySound(GameSoundsData.Sound.PlayerHurt, transform.position);
        // CinemachineCameraShake.Instance.ShakeCamera(1.5f, 0.3f);
        StartCoroutine(OnHitCoroutine());
    }

    private IEnumerator OnHitCoroutine() {
        //spriteFlasher.MultiFlash(15, 3);
        healthSystem.SetInvulnerability(true);
        yield return new WaitForSeconds(3);
        healthSystem.SetInvulnerability(false);
    }

    private void PlayerDead()
    {
        // ScoreManager.Instance.SetScore(killCount);
        LoadGameOver();
    }

    private void LoadGameOver()
    {
        // SceneLoader.Load(SceneLoader.Scene.GameOverScreen);
    }

    public void FlipSpriteToFaceDirection(Vector2 direction)
    {
        if (direction.x == 0) return;
        spriteRenderer.flipX = direction.x < 0f;
        if (direction.x > 0)
        {
            IsFacingRight = true;
        }
        else
        {
            IsFacingRight = false;
        }
    }

    public void SetVelocity(Vector2 velocity)
    {
        Vector3 newVelocity =  new Vector3(velocity.x, 0, velocity.y);
        rigidbody.linearVelocity = newVelocity;
    }
    
    public enum AnimationName {
        PlayerIdle,
        PlayerRoll,
        PlayerRun,
        PlayerAttack1,
        PlayerAttack2,
        PlayerAttack3,
        PlayerReturnToIdle
    }
    public void SetAnimation(AnimationName animation)
    {
        animator.Play(animation.ToString());
    }

    public void addKill()
    {
        killCount++;
    }

    public int getKills()
    {
        return killCount;
    }

    // ---------- Event Listeners ---------- //

    private void HandleRoll()
    {
        rollInputDown = true;
    }

    private void HandleMove(Vector2 movement)
    {
        movementInput = movement;
    }

    private void HandleAttack()
    {
        attackInputDown = true;
    }  
    
    private void HandleLook(Vector2 lookInput) {
        this.lookInput = lookInput;
    }

    // ---------- Debugging ---------- //

    private void DebugDrawLookInput() {
        UnityEngine.Debug.DrawLine(
            transform.position,
            transform.position + 5f*new Vector3(lookInput.x, 0, lookInput.y),
            Color.blue,
            .05f
        );
    }
}
}