using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Vocabattle.Game
{
    public class PlayerTurnUI : MonoBehaviour
    {
        public Text infoText;
        public InputField wordInputField;
        public Text errorText;
        private Dictionary<string, object> words;
        private List<string> usedWords;
        private string targetLetter;
        private const string InfoPrefix = "Create a word with the letter";

        public void SetWords(Dictionary<string, object> words)
        {
            this.words = words;
        }

        public void SetUsedWords(List<string> usedWords)
        {
            this.usedWords = usedWords;
        }

        public void Show(string letter)
        {
            targetLetter = letter;
            infoText.text = $"{InfoPrefix} \"{letter}\"";
            wordInputField.text = string.Empty;
            errorText.text = string.Empty;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            errorText.text = string.Empty;
            infoText.text = string.Empty;
        }

        public void OnSubmit()
        {
            ValidateAndSubmitWord(wordInputField.text);
        }

        private void ValidateAndSubmitWord(string word)
        {
            if (string.IsNullOrEmpty(word))
            {
                errorText.text = $"You cannot submit if the field is blank!";
                return;
            }

            if (word.Contains(" "))
            {
                errorText.text = $"You cannot submit if you have spaces!";
                return;
            }

            if (!word.ToLower().StartsWith(targetLetter))
            {
                errorText.text = $"Your word does not start with the letter {targetLetter}!";
                return;
            }

            if (usedWords.Contains(word.Trim().ToLower()))
            {
                errorText.text = $"Your word has been already been used in this game!";
                return;
            }

            if (!words.ContainsKey(word.Trim().ToLower()))
            {
                errorText.text = $"You probably have typed an invalid word!";
                return;
            }

            EventManager.Instance.TriggerEvent(new SubmitWordEvent(word.Trim().ToLower()));
        }
    }
}