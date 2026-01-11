using UnityEngine;

public class TapToReveal : MonoBehaviour
{
    [Header("Assign in Inspector")]
    public ParticleSystem revealParticles;
    public Transform targetToPop;

    private bool revealed = false;
    private Vector3 baseScale;

    void Start()
    {
        if (targetToPop == null) targetToPop = transform;
        baseScale = targetToPop.localScale;
    }

    void Update()
    {
        if (Input.touchCount == 0) return;

        Touch t = Input.GetTouch(0);
        if (t.phase != TouchPhase.Began) return;

        Ray ray = Camera.main.ScreenPointToRay(t.position);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Did we tap THIS object (or a child)?
            if (hit.transform == transform || hit.transform.IsChildOf(transform))
            {
                ToggleReveal();
            }
        }
    }

    void ToggleReveal()
    {
        revealed = !revealed;

        if (revealParticles != null)
        {
            if (revealed) revealParticles.Play();
            else revealParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        // Quick pop effect
        StopAllCoroutines();
        StartCoroutine(PopAnimation());
    }

    System.Collections.IEnumerator PopAnimation()
    {
        float t = 0f;
        float dur = 0.12f;
        Vector3 big = baseScale * 1.2f;

        while (t < dur)
        {
            t += Time.deltaTime;
            targetToPop.localScale = Vector3.Lerp(baseScale, big, t / dur);
            yield return null;
        }

        t = 0f;
        while (t < dur)
        {
            t += Time.deltaTime;
            targetToPop.localScale = Vector3.Lerp(big, baseScale, t / dur);
            yield return null;
        }
    }
}
