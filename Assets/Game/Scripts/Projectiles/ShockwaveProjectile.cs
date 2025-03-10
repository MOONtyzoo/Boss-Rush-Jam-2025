using UnityEngine;

public class ShockwaveProjectile : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Tooltip("The projectile will destroy itself on colliding with any of these layers")]
    [SerializeField] private LayerMask destroySelfLayerMask;
    [SerializeField] private LayerMask damageLayerMask;

    private new Rigidbody2D rigidbody;

    private float moveSpeed;
    private int damage = 1;

    private void Awake() {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    public void Initialize(float moveSpeed, Vector2 moveDirection) {
        transform.right = -moveDirection.normalized;
        this.moveSpeed = moveSpeed;
    }

    private void FixedUpdate() {
        rigidbody.linearVelocity = moveSpeed * GetMoveDirection();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (destroySelfLayerMask.Contains(other.gameObject.layer)) {
            Destroy(gameObject);
        }

        if (damageLayerMask.Contains(other.gameObject.layer)) {
            HealthSystem healthSystem = other.attachedRigidbody.gameObject.GetComponent<HealthSystem>();
            healthSystem?.Damage(damage);
        }
    }

    private Vector2 GetMoveDirection() {
        return -transform.right;
    }
}
