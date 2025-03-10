using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/*
    See https://www.ronja-tutorials.com/post/048-material-property-blocks/

    The script is set up this way to allow altering the material properties
    on a per instance basis instead of it applying to everything using the material.
*/

public class SpriteEffectsPropertySetter : MonoBehaviour
{
    [SerializeField] private Color TintColor = Color.white;

    [Range(0f, 1f)]
    [SerializeField] private float TintAmount;

    [Range(0f, 6.28318531f)]
    [SerializeField] private float HueShift;

    [Range(0f, 5f)]
    [SerializeField] private float Saturation = 1;

    [Range(-1f, 1f)]
    [SerializeField] private float Brightness;

    [Range(0f, 1f)]
    [SerializeField] private float Opacity = 1f;

    private new Renderer renderer;
    private Material material;

    // OnValidate is called in the editor after the component is edited
    private void OnValidate() {
        // renderer = GetComponent<Renderer>();
        // if (material == null) CopyMaterialForThisInstance();
        // UpdateShaderProperties();
    }

    private void Awake() {
        renderer = GetComponent<Renderer>();
        if(!renderer.material.name.Contains("SpriteEffects")) {
            Debug.LogError("This object has a SpriteEffectsPropertySetter, but the material was not set to SpriteEffects, so it won't work", this);
            return;
        }
        if (material == null) CopyMaterialForThisInstance();
        UpdateShaderProperties();
    }

    private void UpdateShaderProperties()
    {
        material.SetColor("_TintColor", TintColor);
        material.SetFloat("_TintAmount", TintAmount);
        material.SetFloat("_HueShift", HueShift);
        material.SetFloat("_Saturation", Saturation);
        material.SetFloat("_Brightness", Brightness);
        material.SetFloat("_Opacity", Opacity);
    }

    private void CopyMaterialForThisInstance() {
        material = new Material(renderer.sharedMaterial);
        renderer.material = material;
    }

    public void SetTintColor(Color newTintColor) {
        TintColor = newTintColor;
        
        renderer.material.SetColor("_TintColor", TintColor);
    }

    public void SetTintAmount(float newTintAmount) {
        TintAmount = newTintAmount;

        renderer.material.SetFloat("_TintAmount", TintAmount);
    }

    public void SetHueShift(float newHueShift) {
        HueShift = newHueShift;

        renderer.material.SetFloat("_HueShift", HueShift);
    }

    public void SetSaturation(float newSaturation) {
        Saturation = newSaturation;

        renderer.material.SetFloat("_Saturation", Saturation);
    }

    public void SetBrightness(float newBrightness) {
        Brightness = newBrightness;

        renderer.material.SetFloat("_Brightness", Brightness);
    }

    public void SetOpacity(float newOpacity) {
        Opacity = newOpacity;

        renderer.material.SetFloat("_Opacity", Opacity);
    }
}
