using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using BRJ.SceneManagement;
using System.Collections.Generic;

public class BRJLoadingManager : MonoBehaviour
{
    [SerializeField] private TMP_Text tooltipText;
    [SerializeField] private Slider loadingBar; // Reference to the slider for loading
    [SerializeField] private RectTransform handleTransform; // Reference to the handle
    [SerializeField] private Animator handleAnimator; // Reference to the Animator component
    [Header("Settings")]
    [SerializeField] private List<string> tooltips;

    private bool isAnimatingHandle = true; // To control handle movement during loading

    // Animator Trigger or Bool for controlling animation
    private const string ANIMATOR_BOOL_ISLOADING = "IsLoading"; // This is the new parameter for control

    void Awake()
    {
        tooltipText.text = GetRandomTooltip();
        StartCoroutine(LoadNextScene());
    }

    private string GetRandomTooltip()
    {
        return tooltips[Random.Range(0, tooltips.Count)];
    }

    private IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(Random.Range(1f, 2f)); // Fake loading delay

        if (BRJSceneManager.SceneToLoad != null)
        {
            string sceneName = BRJSceneManager.SceneToLoad;
            BRJSceneManager.ClearSceneToLoad();

            // Start loading the next scene asynchronously
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            asyncLoad.allowSceneActivation = false; // Prevent the scene from activating until we decide

            if (asyncLoad != null)
            {
                // Play animation through Animator when loading starts
                if (handleAnimator != null)
                {
                    handleAnimator.SetBool(ANIMATOR_BOOL_ISLOADING, true); // Start the loading animation
                }

                // Update slider and handle as the scene loads
                while (asyncLoad.progress < 0.9f) // Unity loads up to 90%
                {
                    float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f); // Normalize progress
                    loadingBar.value = progress; // Update slider value to match loading progress

                    // Update the handle's position in sync with the slider
                    UpdateHandlePosition();

                    yield return null; // Wait for the next frame
                }

                // Loading is complete; smoothly fill the last 10%
                loadingBar.value = 1f; // Fill the slider to 100%
                yield return new WaitForSeconds(0.5f); // Optional: Small delay to show full bar

                asyncLoad.allowSceneActivation = true; // Activate the scene

                // Stop animation through Animator when loading is complete
                if (handleAnimator != null)
                {
                    handleAnimator.SetBool(ANIMATOR_BOOL_ISLOADING, false); // Stop the loading animation
                }
            }
            else
            {
                FallbackSceneTransition();
            }
        }
        else
        {
            FallbackSceneTransition();
        }
    }

    private void FallbackSceneTransition()
    {
        Debug.LogError("[BRJ ERROR] Failed to load scene. Falling back to build index 0.");
        SceneManager.LoadSceneAsync(0);
    }

    // Updates the handle's position to follow the slider value
    private void UpdateHandlePosition()
    {
        if (loadingBar.fillRect != null)
        {
            // Get the slider's fill area and move the handle within it
            RectTransform fillArea = loadingBar.fillRect;
            float handleX = Mathf.Lerp(fillArea.rect.xMin, fillArea.rect.xMax, loadingBar.value); // Calculate the X position
            Vector3 newPosition = new Vector3(handleX, handleTransform.localPosition.y, handleTransform.localPosition.z);
            handleTransform.localPosition = newPosition; // Update the handle's position
        }
    }
}