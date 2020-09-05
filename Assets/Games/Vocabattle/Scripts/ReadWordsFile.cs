using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace Game.General
{
    public class ReadWordsFile : MonoBehaviour
    {
        public TextAsset wordsText;
        private Dictionary<string, object> words;

        // Start is called before the first frame update
        private IEnumerator Start()
        {
            string json = wordsText.text;
            yield return null;
            words = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            EventManager.Instance.TriggerEvent(new WordsLoadedEvent(words));
        }
    }
}