using UnityEngine;

/* This script allows an object to be revealed or hidden when the user taps it on the screen. The interaction uses touch input and raycasting to detect taps on the object or its children.
 When triggered, the script:
 - Toggles a particle effect
 - Plays a short "pop" scale animation for visual feedback*/
public class TapToReveal : MonoBehaviour
{
    [Header("Assign in Inspector")]

    // Particle effect that plays when the object is revealed
    public ParticleSystem revealParticles;

    // Transform that performs the pop animation 
    public Transform targetToPop;

    // Tracks whether the object is currently revealed
    private bool revealed = false;

    // Stores the original scale for animation resets
    private Vector3 baseScale;

    // Called once when the object becomes active
    void Start()
    {
        // Fallback to this transform if no target is assigned
        if (targetToPop == null)
            targetToPop = transform;

        // Store the initial scale for use in the pop animation
        baseScale = targetToPop.localScale;
    }

    // Called once per frame
    void Update()
    {
        // Exit early if no touch input is detected
        if (Input.touchCount == 0) return;

        // Get the first touch
        Touch t = Input.GetTouch(0);

        // Only respond when the touch begins
        if (t.phase != TouchPhase.Began) return;

        // Cast a ray from the camera through the touch position
        Ray ray = Camera.main.ScreenPointToRay(t.position);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Check if the tapped object is this object or one of its children
            if (hit.transform == transform || hit.transform.IsChildOf(transform))
            {
                ToggleReveal();
            }
        }
    }

    /* Switches between revealed and hidden states. Controls the particle system and triggers the pop animation. */
    void ToggleReveal()
    {
        // Toggle reveal state
        revealed = !revealed;

        // Play or stop particle effects depending on state
        if (revealParticles != null)
        {
            if (revealed)
                revealParticles.Play();
            else
                revealParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        // Restart pop animation to prevent overlapping coroutines
        StopAllCoroutines();
        StartCoroutine(PopAnimation());
    }

    /* Performs a short scale-up and scale-down animation to give visual feedback when the object is tapped. */
    System.Collections.IEnumerator PopAnimation()
    {
        float t = 0f;
        float dur = 0.12f;

        // Target scale for the pop effect
        Vector3 big = baseScale * 1.2f;

        // Scale up
        while (t < dur)
        {
            t += Time.deltaTime;
            targetToPop.localScale = Vector3.Lerp(baseScale, big, t / dur);
            yield return null;
        }

        t = 0f;

        // Scale back down
        while (t < dur)
        {
            t += Time.deltaTime;
            targetToPop.localScale = Vector3.Lerp(big, baseScale, t / dur);
            yield return null;
        }

        // Ensure the final scale is exact
        targetToPop.localScale = baseScale;
    }
}
