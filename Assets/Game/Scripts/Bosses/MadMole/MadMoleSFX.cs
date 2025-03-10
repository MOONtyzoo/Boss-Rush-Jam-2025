using UnityEngine;

public class MadMoleSFX : MonoBehaviour
{
    [SerializeField] private MadMole madMole;

    private void Start() {
        madMole.OnRoarStarted += () => AudioManager.PlaySound(SoundEffects.MadMoleRoar);
        madMole.OnJumped += () => AudioManager.PlaySound(SoundEffects.MadMoleJump);
        madMole.OnJumpLanded += () => AudioManager.PlaySound(SoundEffects.MadMoleLand);
        madMole.OnStomped += () => AudioManager.PlaySound(SoundEffects.MadMoleLand);
        madMole.OnSuperJumped += () => AudioManager.PlaySound(SoundEffects.MadMoleSpin);
        madMole.OnSuperFallEnded += () => AudioManager.PlaySound(SoundEffects.MadMoleSuperLand);
    }
}
