using UnityEngine;

public class DummyBoss : MonoBehaviour
{
    private HealthSystem healthSystem;

    private void Awake() {
        healthSystem = GetComponent<HealthSystem>();

        healthSystem.OnHealthDepleted += HealthSystem_OnHealthDepleted;
    }

    private void HealthSystem_OnHealthDepleted() {
        Destroy(gameObject);
    }
}
