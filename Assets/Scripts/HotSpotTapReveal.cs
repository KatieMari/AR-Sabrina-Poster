using UnityEngine;

public class HotspotTapReveal : MonoBehaviour
{
    [Header("Assign in Inspector")]
    public Camera arCamera;                 // AR Camera
    public Collider hotspotCollider;         // Hotspot_TapToReveal collider
    public ParticleSystem revealParticles;   // Heart/sparkle particles
    public Transform targetToPop;            // Text (optional)
    public AudioSource musicSource;          // AudioSource on ARPosterContent

    [Header("Tuning")]
    public float popScale = 1.2f;
    public float popDuration = 0.12f;

    private Vector3 baseScale;
    private bool revealed = false;

    void Start()
    {
        if (arCamera == null) arCamera = Camera.main;
        if (targetToPop == null) targetToPop = transform;

        baseScale = targetToPop.localScale;
    }

    void Update()
    {
        if (Input.touchCount == 0) return;

        Touch t = Input.GetTouch(0);
        if (t.phase != TouchPhase.Began) return;

        Ray ray = arCamera.ScreenPointToRay(t.position);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Only trigger if we tapped the hotspot (or its child)
            if (hotspotCollider != null && (hit.collider == hotspotCollider))
            {
                ToggleReveal();
            }
        }
    }

    void ToggleReveal()
    {
        revealed = !revealed;

        // Particles
        if (revealParticles != null)
        {
            if (revealed) revealParticles.Play();
            else revealParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        // Music
        if (musicSource != null)
        {
            if (revealed && !musicSource.isPlaying) musicSource.Play();
            if (!revealed && musicSource.isPlaying) musicSource.Stop();
        }

        // Pop animation
        StopAllCoroutines();
        StartCoroutine(PopRoutine());
    }

    System.Collections.IEnumerator PopRoutine()
    {
        float t = 0f;
        Vector3 big = baseScale * popScale;

        while (t < popDuration)
        {
            t += Time.deltaTime;
            targetToPop.localScale = Vector3.Lerp(baseScale, big, t / popDuration);
            yield return null;
        }

        t = 0f;
        while (t < popDuration)
        {
            t += Time.deltaTime;
            targetToPop.localScale = Vector3.Lerp(big, baseScale, t / popDuration);
            yield return null;
        }

        targetToPop.localScale = baseScale;
    }
}
