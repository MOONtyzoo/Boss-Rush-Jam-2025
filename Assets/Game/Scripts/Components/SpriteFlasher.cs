using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

/*
    TODO: Reprogram this script to work with LeanTween (or no tweens)
*/

[RequireComponent(typeof(SpriteEffectsPropertySetter))]
public class SpriteFlasher : MonoBehaviour
{
    [SerializeField] private Color32 flashColor = Color.white;
    private SpriteEffectsPropertySetter spriteEffectsPropertySetter;

    public enum Transition {
        Constant,
        Linear,
    }

    public void Awake() {
        spriteEffectsPropertySetter = GetComponent<SpriteEffectsPropertySetter>();
    }

    public void SingleFlash(float duration, Transition transition = Transition.Constant) {
        StartCoroutine(SingleFlashCoroutine(duration, transition));
    }

    public IEnumerator SingleFlashCoroutine(float duration, Transition transition) {
        float timer = 0;
        float progress = 0;

        spriteEffectsPropertySetter.SetTintColor(flashColor);
        while (timer < duration) {
            timer += Time.deltaTime;
            progress = timer/duration;
            spriteEffectsPropertySetter.SetTintAmount(GetTintAmount(progress, transition));
            yield return null;
        }
        spriteEffectsPropertySetter.SetTintAmount(0);
    }

    private float GetTintAmount(float progress, Transition transitionType) {
        switch(transitionType) {
            case Transition.Constant:
                return 1f;
            case Transition.Linear:
                return 1f-progress;
            default:
                return 1f;
        }
    }
}
