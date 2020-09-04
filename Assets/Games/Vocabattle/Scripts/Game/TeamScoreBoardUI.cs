using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Vocabattle.Game
{
    public class TeamScoreBoardUI : MonoBehaviour
    {
        public Text teamNameText;
        public RectTransform wordsContainer;
        public Text wordTextPrefab;
        public Text scoreText;
        private List<Text> wordTexts;
        private const string ScorePrefix = "Score:";

        public void Initialize(string teamName)
        {
            teamNameText.text = teamName;
            scoreText.text = $"{ScorePrefix} 0";

            wordTexts = new List<Text>();
        }

        public void SetScore(int score)
        {
            scoreText.text = $"{ScorePrefix} {score.ToString()}";
        }

        public void ShowWords(List<string> words)
        {
            ClearWords();

            foreach (var word in words)
            {
                Text wordText = Instantiate(wordTextPrefab, wordsContainer);
                wordText.text = $"{word} ({word.Length})";
                wordTexts.Add(wordText);
            }
        }

        private void ClearWords()
        {
            foreach(var wordText in wordTexts)
            {
                Destroy(wordText.gameObject);
            }

            wordTexts = new List<Text>();
        }
    }
}