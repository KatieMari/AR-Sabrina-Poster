using UnityEngine;
using TMPro;

public class DebugTextFinder : MonoBehaviour
{
    void Start()
    {
        var texts = FindObjectsOfType<TMP_Text>(true);
        Debug.Log($"[DebugTextFinder] TMP_Text found: {texts.Length}");

        foreach (var t in texts)
            Debug.Log($"[TMP] '{t.text}' | {GetPath(t.transform)} | {t.GetType().Name}");
    }

    static string GetPath(Transform t)
    {
        string path = t.name;
        while (t.parent != null)
        {
            t = t.parent;
            path = t.name + "/" + path;
        }
        return path;
    }
}

