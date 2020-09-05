using System.Collections.Generic;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
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
        private string teamName;
        private const string ScorePrefix = "Score:";

        public void Initialize(string teamName, int score)
        {
            this.teamName = teamName;
            teamNameText.text = teamName;
            scoreText.text = $"{ScorePrefix} {score}";

            wordTexts = new List<Text>();
        }

        public void SetScoreBoard(ScoreUpdateEvent.RoundWiseScore roundWiseScore)
        {
            scoreText.text = $"{ScorePrefix} {roundWiseScore.Player.GetScore()}";

            List<string> words = new List<string>();

            for (int i = 0; i < roundWiseScore.MaxRounds; i++)
            {
                string key = $"{VocabattleConstants.Round}_{i + 1}";
                if (!roundWiseScore.Player.CustomProperties.ContainsKey(key))
                {
                    continue;
                }

                string word = GetWord(roundWiseScore.Player, key);

                if(string.IsNullOrEmpty(word))
                {
                    continue;
                }

                words.Add(word);
            }

            ShowWords(words);
        }

        public void SetScoreBoard(Player player, int maxRounds)
        {
            scoreText.text = $"{ScorePrefix} {player.GetScore()}";

            List<string> words = new List<string>();

            for (int i = 0; i < maxRounds; i++)
            {
                string key = $"{VocabattleConstants.Round}_{i + 1}";
                if (!player.CustomProperties.ContainsKey(key))
                {
                    continue;
                }

                string word = GetWord(player, key);

                if(string.IsNullOrEmpty(word))
                {
                    continue;
                }

                words.Add(word);
            }

            ShowWords(words);
        }

        private string GetWord(Player player, string key)
        {
            object word;

            if (player.CustomProperties.TryGetValue(key, out word))
            {
                return (string)word;
            }

            return string.Empty;
        }

        private void ShowWords(List<string> words)
        {
            ClearWords();

            foreach (var word in words)
            {
                Text wordText = Instantiate(wordTextPrefab, wordsContainer);
                wordText.text = $"{word} ({word.Length})";
                wordTexts.Add(wordText);
            }
        }

        public bool IsTeam(string teamName)
        {
            return this.teamName.Equals(teamName);
        }

        private void ClearWords()
        {
            foreach (var wordText in wordTexts)
            {
                Destroy(wordText.gameObject);
            }

            wordTexts = new List<Text>();
        }
    }
}