using UnityEngine;

[CreateAssetMenu(menuName="ScriptableObjects/GameData/MadMoleDataSO", fileName ="MadMoleDataSO")]
public class MadMoleDataSO : ScriptableObject
{
    [Header("General")]

    [SerializeField] public int maxHealth;
    [Tooltip("Layers for any kind of terrain that the movement system interacts with")]
    [SerializeField] public LayerMask terrainLayerMask;
    [SerializeField] public int gravityScale;

    [Header("Idle State")]
    [Tooltip("The avereage time waited in between attacks")]
    [SerializeField] public float decisionTime = 1f;
    
    [Header("Jump State")]
    [SerializeField] public float jumpChargeTime = 0f;
    [SerializeField] public float jumpPeakTime = 0.5f;
    [SerializeField] public float jumpLandTime = 1f;
    [SerializeField] public float jumpStrength;
    [SerializeField] public float jumpUpwardGravityScale = 1f;
    [SerializeField] public float jumpFallGravityScale = 1.5f;
    [SerializeField] public float jumpDistanceMax;

    [Header("Stomp State")]
    [SerializeField] public float stompChargeTime = 0.4f;
    [SerializeField] public ShockwaveProjectile shockwaveProjectilePrefab;
    [SerializeField] public float shockwaveSpeed = 15;

    [Header("Roar State")]
    [SerializeField] public float roarDuration = 1f;

    [Header("Super Jump State")]
    [SerializeField] public float superJumpChargeTime = 0f;
    [SerializeField] public float superJumpPeakTime = 0.5f;
    [SerializeField] public float superJumpFallTime = 0.6f;
    [SerializeField] public float superJumpLandTime = 1f;
    [SerializeField] public float superJumpHeight = 3f;
}
