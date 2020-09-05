using System.Collections.Generic;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Vocabattle.Game
{
    public class GameOverPanel : MonoBehaviour
    {
        public Text victorTeamNameText;
        public TeamScoreBoardUI scoreboardPrefab;
        public RectTransform scoreBoardContainer;
        private List<TeamScoreBoardUI> scoreBoards;

        private void SetVictorMessageText(string victorTeamName)
        {
            victorTeamNameText.text = victorTeamName;
        }

        public void Initialize(GameOverEvent.RoundWiseScore roundWiseScores)
        {
            gameObject.SetActive(true);
            CreateScoreBoards(roundWiseScores);
        }

        private void CreateScoreBoards(GameOverEvent.RoundWiseScore roundWiseScores)
        {
            Player[] players = roundWiseScores.Players;
            CheckWinners(players);
            int rounds = roundWiseScores.MaxRounds;

            scoreBoards = new List<TeamScoreBoardUI>();

            foreach (var player in players)
            {
                TeamScoreBoardUI scoreboard = Instantiate(scoreboardPrefab, scoreBoardContainer);
                scoreboard.Initialize(player.UserId, player.GetScore());
                scoreboard.SetScoreBoard(player, rounds);
                scoreBoards.Add(scoreboard);
            }
        }

        private void CheckWinners(Player[] players)
        {
            int maxScore = 0;
            List<Player> winners = new List<Player>();

            foreach (var player in players)
            {
                if (player.GetScore() > maxScore)
                {
                    maxScore = player.GetScore();
                    winners.Clear();
                    winners.Add(player);
                }
            }

            foreach (var player in players)
            {
                if (winners.Contains(player))
                {
                    continue;
                }

                if (player.GetScore() == maxScore)
                {
                    winners.Add(player);
                }
            }

            if (winners.Count == 0)
            {
                return;
            }

            if (winners.Count == 1)
            {
                SetVictorMessageText(winners[0].UserId);
                return;
            }

            string message = "";
            for (int i = 0; i < winners.Count; i++)
            {
                if (i == winners.Count - 1)
                {
                    message += " & ";
                }
                else if(i > 0)
                {
                    message += ", ";
                }

                message += winners[i].UserId;
            }

            SetVictorMessageText(message);
        }
    }
}