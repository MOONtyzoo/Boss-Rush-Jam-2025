using UnityEngine;

/// <summary>
/// <para>The purpose of this class is to handle spawning bullet projectiles for the player.</para>
/// </summary>
public class PlayerWebShooter : MonoBehaviour
{
    [SerializeField] private PrototypePlayerDataSO playerData;

    [SerializeField] private WebProjectile projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;

    private float cooldownTimer;

    private void Start() {
        cooldownTimer = playerData.webShooterCooldown;
    }

    public void Shoot(Vector2 targetPosition) {
        WebProjectile projectile = Instantiate(projectilePrefab);
        projectile.transform.position = projectileSpawnPoint.position;
        projectile.Initialize(playerData.projectileDamage, playerData.projectileSpeed, targetPosition);
        cooldownTimer = 0;
    }

    private void Update() {
        if (cooldownTimer < playerData.webShooterCooldown) {
            cooldownTimer += Time.deltaTime;
        }
    }

    public bool CanShoot() => cooldownTimer >= playerData.webShooterCooldown;
}
