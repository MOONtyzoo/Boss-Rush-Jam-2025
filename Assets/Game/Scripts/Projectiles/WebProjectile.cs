using UnityEngine;

public class WebProjectile : MonoBehaviour
{
    [SerializeField] private Transform projectileVisual;
    [Tooltip("The projectile will destroy itself on colliding with any of these layers")]
    [SerializeField] private LayerMask destroySelfLayerMask;
    [SerializeField] private LayerMask damageLayerMask;

    [SerializeField] private ParticleSystem hitTerrainParticles;
    [SerializeField] private ParticleSystem hitEnemyParticles;


    private new Rigidbody2D rigidbody;

    private float moveSpeed;
    private int damage;

    private void Awake() {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    public void Initialize(int damage, float moveSpeed, Vector2 targetPosition) {
        Vector2 initialMoveDirection = (targetPosition - (Vector2)transform.position).normalized;
        transform.up = initialMoveDirection;
        this.damage = damage;
        this.moveSpeed = moveSpeed;
    }

    private void FixedUpdate() {
        rigidbody.linearVelocity = moveSpeed * GetMoveDirection();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (destroySelfLayerMask.Contains(other.gameObject.layer)) {
            SpawnHitParticles(hitTerrainParticles);
            Destroy(gameObject);
        }

        if (damageLayerMask.Contains(other.gameObject.layer)) {
            HealthSystem healthSystem = other.attachedRigidbody.gameObject.GetComponent<HealthSystem>();
            healthSystem?.Damage(damage);
            SpawnHitParticles(hitEnemyParticles);
            Destroy(gameObject);
        }
    }

    private Vector2 GetMoveDirection() {
        return transform.up;
    }

    private void SpawnHitParticles(ParticleSystem particles) {
        particles.gameObject.transform.SetParent(null);
        particles.Play();
    }
}
