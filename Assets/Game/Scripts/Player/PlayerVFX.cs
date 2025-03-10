using UnityEngine;

public class PlayerVFX : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private ParticleSystem walkingParticles;
    [SerializeField] private ParticleSystem jumpParticles;
    [SerializeField] private ParticleSystem shootParticles;
    [SerializeField] private ParticleSystem grapplingParticles;

    private void Update()
    {
        HandleWalkingParticles();
        HandleGrapplingParticles();
    }

    private void Start() {
        player.OnJumped += () => {
            jumpParticles.Stop();
            jumpParticles.Play();
        };
        player.OnWallJumped += () => {
            jumpParticles.Stop();
            jumpParticles.Play();
        };

        player.OnShotGun += () => {
            shootParticles.Stop();
            shootParticles.Play();
        };
    }

    private void HandleWalkingParticles() {
        // It's a bit dirty, but this logic handles walking on ground and sliding down walls
        if ((player.IsGrounded() || player.IsTouchingWall()) && (player.IsMoving() || player.IsFalling())) {
            if (!walkingParticles.isPlaying) walkingParticles.Play();
        } else {
            if (walkingParticles.isPlaying) walkingParticles.Stop();
        }
    }

    private void HandleGrapplingParticles() {
        if (player.IsGrappling()) {
            if (!grapplingParticles.isPlaying) grapplingParticles.Play();
        } else {
            if (grapplingParticles.isPlaying) grapplingParticles.Stop();
        }
    }
}
