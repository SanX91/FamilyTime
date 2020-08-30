using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class ReadWordsFile : MonoBehaviour
{
    public string word;
    public TextAsset wordsText;

    // Start is called before the first frame update
    private IEnumerator Start()
    {
        DateTime startTime = DateTime.UtcNow;
        string json = wordsText.text;
        Debug.Log($"Step 1: {(DateTime.UtcNow - startTime).TotalMilliseconds} ms");

        yield return null;

        startTime = DateTime.UtcNow;
        Dictionary<string, object> words = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
        Debug.Log($"Step 2: {(DateTime.UtcNow - startTime).TotalMilliseconds} ms");

        yield return null;

        Debug.Log($"Words count: {words.Count}");

        startTime = DateTime.UtcNow;
        Debug.Log($"Found word: {words.ContainsKey(word?.ToLower())}");
        Debug.Log($"Step 3: {(DateTime.UtcNow - startTime).TotalMilliseconds} ms");
    }
}
