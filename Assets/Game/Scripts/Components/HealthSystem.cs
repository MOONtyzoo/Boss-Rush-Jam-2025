using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class HealthSystem : MonoBehaviour
{
    private int health = 100;
    [SerializeField] private int maxHealth = 100;
    private bool isInvulnerable = false;

    [Space]
    public Action OnHealthChanged;
    public Action OnDamaged;
    public Action OnHealed;
    public Action OnHealthDepleted;

    void Awake() {
        health = maxHealth;
    }

    public void Damage(int damageNum) {
        if (isInvulnerable) return;
        SetHealth(health - damageNum);
        OnDamaged?.Invoke();
    }

    public void Heal(int healingNum) {
        SetHealth(health + healingNum);
        OnHealed?.Invoke();
    }

    public void SetHealth(int newHealth) {
        newHealth = Mathf.Clamp(newHealth, 0, maxHealth);
        if (health != newHealth) {
            health = newHealth;
            OnHealthChanged?.Invoke();
            if (IsHealthDepleted()) OnHealthDepleted?.Invoke();
        }
    }

    public void SetMaxHealth(int newMaxHealth) {
        newMaxHealth = Mathf.Max(newMaxHealth, 0);
        health = Mathf.Clamp(health, 0, newMaxHealth);
        maxHealth = newMaxHealth;
        OnHealthChanged?.Invoke();
    }

    public bool IsHealthDepleted() {
        return health == 0;
    }

    public int GetHealth() {
        return health;
    }

    public int GetMaxHealth() {
        return maxHealth;
    }

    public float GetHealthPercentage() {
        return ((float)health)/maxHealth;
    }

    public override string ToString() {
        return health.ToString() + "/" + maxHealth.ToString();
    }

    public void SetInvulnerability(bool isInvulnerable) {
        this.isInvulnerable = isInvulnerable;
    }
}
