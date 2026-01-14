using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class TrackedImageSpawner : MonoBehaviour
{
    [Header("Assign in Inspector")]
    public ARTrackedImageManager trackedImageManager;
    public GameObject contentPrefab;

    private readonly Dictionary<string, GameObject> spawned = new();

    void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach (var img in args.added) UpdateImage(img);
        foreach (var img in args.updated) UpdateImage(img);
        foreach (var img in args.removed) RemoveImage(img);
    }

    private void UpdateImage(ARTrackedImage img)
    {
        string key = img.referenceImage.name;

        if (!spawned.ContainsKey(key))
        {
            // Parent to the tracked image so it stays locked to the poster
            var go = Instantiate(contentPrefab, img.transform);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            spawned.Add(key, go);
        }

        bool isTracking = img.trackingState == TrackingState.Tracking;
        spawned[key].SetActive(isTracking);

        // Keep aligned 
        spawned[key].transform.SetPositionAndRotation(img.transform.position, img.transform.rotation);
    }

    private void RemoveImage(ARTrackedImage img)
    {
        string key = img.referenceImage.name;
        if (spawned.TryGetValue(key, out var go))
        {
            Destroy(go);
            spawned.Remove(key);
        }
    }
}
