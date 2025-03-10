using System;
using System.Collections.Generic;
using UnityEngine;

public class SpriteTrail : MonoBehaviour
{
    [SerializeField] private bool isActive;
    [SerializeField] private SpriteRenderer sourceSpriteRenderer;
    [SerializeField] private Transform spriteGhostParent;
    [SerializeField] private float spriteGhostsPerSecond;
    [SerializeField] private float spriteGhostDuration;

    float ghostSpawnTimer = 0;
    List<SpriteGhost> spriteGhosts = new List<SpriteGhost>();

    private void Update() {
        if (isActive) {
            HandleSpawningSpriteGhosts();
        }
        HandleSpriteGhostLifetimes();
    }

    private void HandleSpawningSpriteGhosts() {
        ghostSpawnTimer += Time.deltaTime;
        if (ghostSpawnTimer > GetGhostSpawnTimerDuration()) {
            ghostSpawnTimer -= GetGhostSpawnTimerDuration();
            SpawnSpriteGhost();
        }
    }

    private void SpawnSpriteGhost() {
        SpriteGhost spriteGhost = new SpriteGhost();
        spriteGhost.Initialize(sourceSpriteRenderer, spriteGhostParent);
        spriteGhosts.Add(spriteGhost);
    }

    private void HandleSpriteGhostLifetimes() {
        List<SpriteGhost> ghostsToDestroy =  new List<SpriteGhost>();
        foreach (SpriteGhost spriteGhost in spriteGhosts) {
            spriteGhost.lifeTime += Time.deltaTime;
            if (spriteGhost.lifeTime > spriteGhostDuration) {
                ghostsToDestroy.Add(spriteGhost);
            } else {
                float progress = spriteGhost.lifeTime/spriteGhostDuration;
                spriteGhost.UpdatePosition();
                spriteGhost.SetOpacity(1f-progress);
            }
        }

        foreach (SpriteGhost spriteGhost in ghostsToDestroy) {
            DestroySpriteGhost(spriteGhost);
        }
    }

    private void DestroySpriteGhost(SpriteGhost spriteGhost) {
        spriteGhosts.Remove(spriteGhost);
        Destroy(spriteGhost.gameObject);
    }

    public void Clear() {
        while (spriteGhosts.Count > 0) {
            DestroySpriteGhost(spriteGhosts[0]);
        }
    }

    public bool IsActive() => isActive;
    public bool SetActive(bool active) => isActive = active;
    public void SetGhostsPerSecond(float newGhostsPerSecond) => spriteGhostsPerSecond = newGhostsPerSecond;
    public void SetGhostDuration(float newGhostDuration) => spriteGhostDuration = newGhostDuration;
    private float GetGhostSpawnTimerDuration() => 1f/spriteGhostsPerSecond;
    
    private class SpriteGhost {
        public float lifeTime;
        public GameObject gameObject;
        public SpriteRenderer spriteRenderer;

        private Vector3 position;
        private Quaternion rotation;
        private Vector3 scale;

        public void Initialize(SpriteRenderer sourceSpriteRenderer, Transform parent) {
            gameObject = new GameObject();
            gameObject.name = "SpriteGhost";
            position = sourceSpriteRenderer.gameObject.transform.position;
            rotation = sourceSpriteRenderer.gameObject.transform.rotation;
            scale = sourceSpriteRenderer.gameObject.transform.localScale;
            gameObject.transform.parent = parent;

            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = sourceSpriteRenderer.sprite;
            spriteRenderer.flipX = sourceSpriteRenderer.flipX;
            spriteRenderer.flipY = sourceSpriteRenderer.flipY;
            spriteRenderer.sortingOrder = sourceSpriteRenderer.sortingOrder - 1;

            lifeTime = 0;
        }

        public void UpdatePosition() {
            gameObject.transform.position = position;
            gameObject.transform.rotation = rotation;
            gameObject.transform.localScale = scale;
        }

        public void SetOpacity(float opacity) {
            Color color = spriteRenderer.color;
            color.a = opacity;
            spriteRenderer.color = color;
        }
    }
}