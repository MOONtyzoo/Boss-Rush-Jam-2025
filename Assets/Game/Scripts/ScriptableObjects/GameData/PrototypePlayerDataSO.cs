using UnityEngine;

[CreateAssetMenu(menuName="ScriptableObjects/GameData/PrototypePlayerDataSO", fileName ="PrototypePlayerDataSO")]
public class PrototypePlayerDataSO : ScriptableObject
{
    [Header("Health")]
    
    [Tooltip("Layers for any kind of stuff that damages the player")]
    [SerializeField] public LayerMask damagingMask;
    [SerializeField] public int maxHealth;
    [SerializeField] public float invulnerabilityDuration;
    [SerializeField] public float hurtStunDuration;
    
    [Header("Movement")]

    [Tooltip("Layers for any kind of terrain that the movement system interacts with")]
    [SerializeField] public LayerMask terrainLayerMask;

    [SerializeField] public float moveSpeed;
    [Tooltip("How fast the player accelerates when moving left/right")]
    [SerializeField] public float groundedAcceleration;
    [Tooltip("How fast the player decelerates to zero")]
    [SerializeField] public float groundedDeceleration;
    [Tooltip("How fast the player accelerates when moving left/right")]
    [SerializeField] public float aerialAcceleration;
    [Tooltip("How fast the player decelerates to zero")]
    [SerializeField] public float aerialDeceleration;
    
    [Tooltip("Friction applied when sliding down wall")]
    [SerializeField] public float wallFriction;

    public float PickMoveAccelerationValue(bool isGrounded, float targetVelocityX) {
        if (isGrounded) {
            if (targetVelocityX != 0) {
                return groundedAcceleration;
            } else {
                return groundedDeceleration;
            }
        } else {
            if (targetVelocityX != 0) {
                return aerialAcceleration;
            } else {
                return aerialDeceleration;
            }
        }
    }



    [Header("Jump")]

    [SerializeField] public float jumpSpeed;
    [SerializeField] public float gravityScale;



    [Header("Web Shooter")]

    [SerializeField] public AimAssisterSO shootAimAssister;

    [SerializeField] public int projectileDamage;
    [SerializeField] public float projectileSpeed;
    [SerializeField] public float webShooterCooldown;


    [Header("Grapple")]

    [SerializeField] public AimAssisterSO grappleAimAssister;

    [Tooltip("The hook will attempt to hook on to objects in these layers")]
    [SerializeField] public LayerMask grapplePointLayerMask;
    [Tooltip("The grapple will retract upon hitting these layers")]
    [SerializeField] public LayerMask grappleObstacleMask;
    
    [Tooltip("How far the hook can shoot out before retracting")]
    [SerializeField] public float grappleRange;
    [Tooltip("How quickly the grapple moves while shooting towards the target position")]
    [SerializeField] public float grappleShootSpeed;
    [Tooltip("How quickly the grapple moves while retracting toward the player")]
    [SerializeField] public float grappleRetractSpeed;

    [Tooltip("How quickly the grapple pulls the player")]
    [SerializeField] public float grapplePullSpeed;
}
