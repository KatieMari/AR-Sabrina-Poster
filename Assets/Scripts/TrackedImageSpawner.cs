using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/* This script listens for image tracking events from ARFoundation and spawns content when a reference image is detected.
 Each tracked image is identified using its unique GUID, ensuring that content is spawned only once per reference image and correctly managed across tracking updates. */
public class TrackedImageSpawner : MonoBehaviour
{
    [Header("Assign in Inspector")]

    // ARTrackedImageManager responsible for detecting reference images
    public ARTrackedImageManager trackedImageManager;

    // Prefab that will be spawned and attached to the tracked image
    public GameObject contentPrefab;

    // Keeps track of spawned content using the reference image GUID as a key
    private readonly Dictionary<string, GameObject> spawned = new();

    // Called when the script is enabled
    void OnEnable()
    {
        // Ensure the tracked image manager has been assigned
        if (trackedImageManager == null)
        {
            Debug.LogError("TrackedImageSpawner: trackedImageManager is not assigned.");
            enabled = false;
            return;
        }

        // Subscribe to AR tracked image change events
        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    // Called when the script is disabled
    void OnDisable()
    {
        // Unsubscribe from the event to avoid memory leaks
        trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    /* Called whenever tracked images are added, updated, or removed. Delegates handling to the appropriate helper methods. */
    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach (var img in args.added)
            UpdateImage(img);

        foreach (var img in args.updated)
            UpdateImage(img);

        foreach (var img in args.removed)
            RemoveImage(img);
    }

    /* Spawns content for newly detected images and updates the visibility of existing content based on tracking state. */
    private void UpdateImage(ARTrackedImage img)
    {
        // Get the unique identifier for the reference image
        var guid = img.referenceImage.guid;

        // Safety check in case the GUID is invalid
        if (guid == System.Guid.Empty)
        {
            Debug.LogWarning("TrackedImageSpawner: referenceImage.guid was empty. Skipping this update.");
            return;
        }

        string key = guid.ToString();

        // Spawn content only once per reference image
        if (!spawned.ContainsKey(key))
        {
            // Instantiate the prefab as a child of the tracked image
            var go = Instantiate(contentPrefab, img.transform);

            // Reset local transform so it aligns with the image
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;

            // Store the spawned object for future reference
            spawned.Add(key, go);

            Debug.Log("Spawned content for guid: " + key);
        }

        // Only show content when tracking is active. This prevents flickering when tracking becomes limited
        bool shouldShow = img.trackingState != TrackingState.None;
        spawned[key].SetActive(shouldShow);
    }

    /* Cleans up spawned content when a tracked image is removed. */
    private void RemoveImage(ARTrackedImage img)
    {
        var guid = img.referenceImage.guid;
        string key = guid.ToString();

        // Destroy and remove the associated content if it exists
        if (spawned.TryGetValue(key, out var go))
        {
            Destroy(go);
            spawned.Remove(key);
        }
    }
}
