using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EntityHealthBar : MonoBehaviour
{
    [SerializeField] private HealthSystem healthSystem;
    [SerializeField] private bool alwaysShow = false;

    private GameObject healthSprite;
    private float healthPercentage = 1.0f;

    private GameObject chippedHealthSprite;
    private float chippedHealthPercentage = 1.0f;
    private float chippedHealthDecayRate = 0.5f;
    private float chippedHealthDecayTimerMax = 0.4f;
    private float chippedHealthDecayTimer = 0.0f;

    private float barLeftEdgePosition;

    void Awake() {
        if (healthSystem == null) {
            Debug.Log("This HealthBar is not reading from any HealthSystem!", this);
        } else {
            healthSystem.OnDamaged += ShowHealthBar;
            healthSystem.OnHealthChanged += () => SetHealthPercentage(healthSystem.GetHealthPercentage());
        }

        healthSprite = transform.Find("HealthSprite").gameObject;
        chippedHealthSprite = transform.Find("ChippedHealthSprite").gameObject;
        
        barLeftEdgePosition = -healthSprite.GetComponent<SpriteRenderer>().size.x/2.0f;

        if (alwaysShow == true) {
            ShowHealthBar();
        } else {
            HideHealthBar();
        }
    }

    void Update() {
        HandleChippedHealthDecay();
    }

    private void HandleChippedHealthDecay() {
        if (chippedHealthDecayTimer > 0.0f) {
            chippedHealthDecayTimer -= Time.deltaTime;
        } else if (chippedHealthPercentage > healthPercentage) {
            float decayedPercentage = chippedHealthPercentage - chippedHealthDecayRate*Time.deltaTime;
            SetChippedHealthPercentage(decayedPercentage);
        }
    }

    private void SetHealthPercentage(float newPercentage) {
        float previousHealthPercentage = healthPercentage;
        healthPercentage = Math.Clamp(newPercentage, 0.0f, 1.0f);
        UpdateBarSprite(healthSprite, healthPercentage);

        chippedHealthDecayTimer = chippedHealthDecayTimerMax;
        if (healthPercentage > chippedHealthPercentage) {
            SetChippedHealthPercentage(healthPercentage);
        } else {
            SetChippedHealthPercentage(previousHealthPercentage);
        }
    }

    private void SetChippedHealthPercentage(float newPercentage) {
        chippedHealthPercentage = Math.Clamp(newPercentage, 0.0f, 1.0f);
        UpdateBarSprite(chippedHealthSprite, chippedHealthPercentage);
    }

    private void UpdateBarSprite(GameObject barSprite, float percentage) {
        Vector3 newLocalPosition = barSprite.transform.localPosition;
        newLocalPosition.x = Mathf.Lerp(barLeftEdgePosition, 0.0f, percentage);

        Vector3 newLocalScale = barSprite.transform.localScale;
        newLocalScale.x = Mathf.Lerp(0.0f, 1.0f, percentage);

        barSprite.transform.localPosition = newLocalPosition;
        barSprite.transform.localScale = newLocalScale;
    }

    private void HideHealthBar() {
        gameObject.SetActive(false);
    }

    private void ShowHealthBar() {
        gameObject.SetActive(true);
    }
}
