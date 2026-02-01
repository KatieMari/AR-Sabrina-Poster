using UnityEngine;

/* This script creates a subtle floating animation by moving the object up and down using a sine wave.*/
public class FloatBob : MonoBehaviour
{
    // Maximum vertical distance the object moves up and down
    public float amplitude = 0.01f;

    // Speed of the bobbing motion
    public float speed = 2f;

    // Stores the original local position of the object
    private Vector3 startPos;

    // Called once when the object becomes active
    void Start()
    {
        // Save the starting position so movement is relative
        startPos = transform.localPosition;
    }

    // Called once per frame
    void Update()
    {
        // Calculate vertical offset using a sine wave over time
        float y = Mathf.Sin(Time.time * speed) * amplitude;

        // Apply the offset to the original position
        transform.localPosition = startPos + new Vector3(0, y, 0);
    }
}
