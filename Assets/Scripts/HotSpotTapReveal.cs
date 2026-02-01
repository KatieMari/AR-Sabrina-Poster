using UnityEngine;

/* This script controls an interactive AR hotspot that toggles a reveal effect when activated. 
The reveal includes:
 - Particle effects
 - Audio playback
 - A short "pop" scale animation */
public class HotspotTapReveal : MonoBehaviour
{
    [Header("Assign in Inspector")]

    // Camera used to raycast from screen touches 
    public Camera arCamera;

    // Collider that defines the interactive hotspot area
    public Collider hotspotCollider;

    // Particle system that plays when the hotspot is revealed
    public ParticleSystem revealParticles;

    // Object that performs the pop scale animation
    public Transform targetToPop;

    // Audio source used to play music or sound on reveal
    public AudioSource musicSource;

    [Header("Tuning")]

    // Maximum scale multiplier for the pop animation
    public float popScale = 1.2f;

    // Duration of each half of the pop animation
    public float popDuration = 0.12f;

    // Stores the original scale of the target object
    private Vector3 baseScale;

    // Tracks whether the hotspot is currently revealed
    private bool revealed = false;

    void Start()
    {
        // Fallback to main camera if AR camera is not assigned
        if (arCamera == null) arCamera = Camera.main;

        // Default to this transform if no target is specified
        if (targetToPop == null) targetToPop = transform;

        // Store the initial scale for animation resets
        baseScale = targetToPop.localScale;
    }



    /* Switches between revealed and hidden states. Controls particles, audio playback, and triggers the pop animation.*/
    public void ToggleReveal()
    {
        Debug.Log("Toggle Reveal triggered");

        // Toggle the reveal state
        revealed = !revealed;

        // Handle particle effects
        if (revealParticles != null)
        {
            if (revealed)
                revealParticles.Play();
            else
                revealParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        // Handle music playback
        if (musicSource != null)
        {
            if (revealed && !musicSource.isPlaying)
                musicSource.Play();

            if (!revealed && musicSource.isPlaying)
                musicSource.Stop();
        }

        // Restart pop animation to avoid overlap
        StopAllCoroutines();
        StartCoroutine(PopRoutine());
    }

    /* Performs a two-stage scale animation:
      1. Scales the object up slightly
      2. Scales it back to its original size
     This provides visual feedback when the hotspot is activated.*/
    System.Collections.IEnumerator PopRoutine()
    {
        float t = 0f;

        // Target scale for the pop effect
        Vector3 big = baseScale * popScale;

        // Scale up
        while (t < popDuration)
        {
            t += Time.deltaTime;
            targetToPop.localScale = Vector3.Lerp(baseScale, big, t / popDuration);
            yield return null;
        }

        t = 0f;

        // Scale back down
        while (t < popDuration)
        {
            t += Time.deltaTime;
            targetToPop.localScale = Vector3.Lerp(big, baseScale, t / popDuration);
            yield return null;
        }

        // Ensure final scale is exact
        targetToPop.localScale = baseScale;
    }
}
