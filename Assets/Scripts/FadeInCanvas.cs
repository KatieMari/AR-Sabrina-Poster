using UnityEngine;
using System.Collections;

/* This script controls a fade-in and fade-out effect for UI elements using a CanvasGroup component.
 * The CanvasGroup alpha value is interpolated over time to create a smooth visual transition.*/
public class FadeInCanvas : MonoBehaviour
{
    // Duration for both fade-in and fade-out transitions
    public float fadeDuration = 0.3f;

    // Reference to the CanvasGroup component attached to this GameObject
    CanvasGroup canvasGroup;

    // Awake is called before Start and is used for initial setup
    void Awake()
    {
        // Get the CanvasGroup component required for fading
        canvasGroup = GetComponent<CanvasGroup>();

        // Start fully transparent
        canvasGroup.alpha = 0f;

        // Enable interaction and raycast blocking while visible
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
    }

    // Start is called on the first frame the object is active
    void Start()
    {
        // Begin fading the canvas in when the scene starts
        StartCoroutine(FadeIn());
    }

    /*Gradually increases the CanvasGroup alpha from 0 to 1over the specified fadeDuration.*/
    IEnumerator FadeIn()
    {
        float time = 0f;

        // Continue fading until the duration is reached
        while (time < fadeDuration)
        {
            // Smoothly interpolate alpha based on elapsed time
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, time / fadeDuration);

            // Increase elapsed time using frame delta
            time += Time.deltaTime;

            // Wait until the next frame
            yield return null;
        }

        // Ensure the canvas is fully visible at the end
        canvasGroup.alpha = 1f;
    }

    /* Public method that can be called by buttons or other scripts to start fading the canvas out. */
    public void FadeOut()
    {
        StartCoroutine(FadeOutRoutine());
    }

    /* Gradually decreases the CanvasGroup alpha from 1 to 0, then disables interaction and deactivates the GameObject.*/
    IEnumerator FadeOutRoutine()
    {
        float time = 0f;

        // Continue fading until the duration is reached
        while (time < fadeDuration)
        {
            // Smoothly interpolate alpha from visible to invisible
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, time / fadeDuration);

            // Increase elapsed time using frame delta
            time += Time.deltaTime;

            // Wait until the next frame
            yield return null;
        }

        // Ensure the canvas is fully invisible
        canvasGroup.alpha = 0f;

        // Disable interaction once the canvas is hidden
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        // Disable the GameObject to fully hide the UI
        gameObject.SetActive(false);
    }
}
