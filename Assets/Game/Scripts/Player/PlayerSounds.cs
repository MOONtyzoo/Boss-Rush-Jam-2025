using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    [Tooltip("This script listens to events in the player to play sounds")]
    [SerializeField] private PlayerController player;

    private void Start() {
        player.OnShotGun += () => AudioManager.PlaySound(SoundEffects.ShootGun);
        player.OnShotGrapple += () => AudioManager.PlaySound(SoundEffects.ShootGrapple);
        player.OnJumped += () => AudioManager.PlaySound(SoundEffects.JumpGround);
        player.OnWallJumped += () => AudioManager.PlaySound(SoundEffects.JumpWall);
        player.OnWallGrabStarted += () => AudioManager.PlaySound(SoundEffects.ShootGrapple);
        player.OnGrappleCanceled += () => AudioManager.PlaySound(SoundEffects.ShootGrapple);
        player.OnHurtStarted += () => AudioManager.PlaySound(SoundEffects.TakeDamage);
    }
}
