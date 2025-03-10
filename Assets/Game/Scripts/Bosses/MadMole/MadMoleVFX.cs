using UnityEngine;

public class MadMoleVFX : MonoBehaviour
{
    [Tooltip("This script listens to events in the boss to choose animations")]
    [SerializeField] private MadMole boss;
    [SerializeField] private SpriteFlasher spriteFlasher;
    [SerializeField] private SpriteTrail spriteTrail;
    [SerializeField] private SpriteEffectsPropertySetter spriteEffectsPropertySetter;

    [SerializeField] private ParticleSystem jumpParticles;
    [SerializeField] private ParticleSystem landParticles;

    private void Start() {
        boss.OnDamaged += () => spriteFlasher.SingleFlash(0.18f, SpriteFlasher.Transition.Linear);
        boss.OnDied += () => {
            spriteFlasher.SingleFlash(0.8f, SpriteFlasher.Transition.Linear);
            spriteEffectsPropertySetter.SetSaturation(0f);
        };

        boss.OnJumped += () => {
            spriteTrail.SetActive(true);
            spriteTrail.SetGhostsPerSecond(20);
            spriteTrail.SetGhostDuration(0.10f);
            jumpParticles.Play();
            CinemachineShake.Instance.ShakeCamera(2f, 0.15f);  
        };
        boss.OnJumpLanded += () => {
            spriteTrail.SetActive(false);
            landParticles.Play();
            CinemachineShake.Instance.ShakeCamera(3.5f, 0.4f);  
        };

        boss.OnRoarStarted += () => {
            CinemachineShake.Instance.ShakeCamera(2f, 1.5f);
        };

        boss.OnStomped += () => {
            CinemachineShake.Instance.ShakeCamera(3.5f, 0.4f);  
        };

        boss.OnSuperJumped += () => {
            spriteTrail.SetActive(true);
            spriteTrail.SetGhostsPerSecond(20);
            spriteTrail.SetGhostDuration(0.25f);
            jumpParticles.Play();
            CinemachineShake.Instance.ShakeCamera(3f, 0.25f);  
        };
        boss.OnSuperFallEnded += () => {
            spriteTrail.Clear();
            spriteTrail.SetActive(false);
        };
    }
}
