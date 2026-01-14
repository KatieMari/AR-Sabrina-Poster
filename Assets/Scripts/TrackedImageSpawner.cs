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
        if (trackedImageManager == null)
        {
            Debug.LogError("TrackedImageSpawner: trackedImageManager is not assigned.");
            enabled = false;
            return;
        }

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

        var guid = img.referenceImage.guid;


        if (guid == System.Guid.Empty)
        {
            Debug.LogWarning("TrackedImageSpawner: referenceImage.guid was empty. Skipping this update.");
            return;
        }

        string key = guid.ToString();

        if (!spawned.ContainsKey(key))
        {
            var go = Instantiate(contentPrefab, img.transform);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            spawned.Add(key, go);

            Debug.Log("Spawned content for guid: " + key);
        }

        // Don't flicker when tracking is Limited
        bool shouldShow = img.trackingState != TrackingState.None;
        spawned[key].SetActive(shouldShow);
    }



    private void RemoveImage(ARTrackedImage img)
    {
        var guid = img.referenceImage.guid;
        string key = guid.ToString();

        if (spawned.TryGetValue(key, out var go))
        {
            Destroy(go);
            spawned.Remove(key);
        }
    }

}
