using UnityEngine;

public class FloatBob : MonoBehaviour
{
    public float amplitude = 0.01f;
    public float speed = 2f;

    private Vector3 startPos;

    void Start() => startPos = transform.localPosition;

    void Update()
    {
        float y = Mathf.Sin(Time.time * speed) * amplitude;
        transform.localPosition = startPos + new Vector3(0, y, 0);
    }
}
