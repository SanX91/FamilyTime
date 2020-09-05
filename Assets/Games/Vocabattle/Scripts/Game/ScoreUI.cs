using System.Collections.Generic;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;

namespace Game.Vocabattle.Game
{
    public class ScoreUI : MonoBehaviour
    {
        public TeamScoreBoardUI scoreboardPrefab;
        public RectTransform scoreBoardContainer;
        private List<TeamScoreBoardUI> scoreBoards;

        private void OnEnable()
        {
            EventManager.Instance.AddListener<CreateScoreBoardsEvent>(OnCreateScoreBoards);
            EventManager.Instance.AddListener<ScoreUpdateEvent>(OnScoreUpdate);
        }

        private void OnDisable()
        {
            EventManager.Instance.RemoveListener<CreateScoreBoardsEvent>(OnCreateScoreBoards);
            EventManager.Instance.RemoveListener<ScoreUpdateEvent>(OnScoreUpdate);
        }

        private void OnCreateScoreBoards(CreateScoreBoardsEvent evt)
        {
            Player[] players = (Player[])evt.GetData();
            scoreBoards = new List<TeamScoreBoardUI>();

            foreach (var player in players)
            {
                TeamScoreBoardUI scoreboard = Instantiate(scoreboardPrefab, scoreBoardContainer);
                scoreboard.Initialize(player.UserId, player.GetScore());
                scoreBoards.Add(scoreboard);
            }
        }

        private void OnScoreUpdate(ScoreUpdateEvent evt)
        {
            ScoreUpdateEvent.RoundWiseScore roundWiseScore = (ScoreUpdateEvent.RoundWiseScore)evt.GetData();
            TeamScoreBoardUI scoreboard = scoreBoards.Find(x => x.IsTeam(roundWiseScore.Player.UserId));

            if (scoreboard == null)
            {
                return;
            }

            scoreboard.SetScoreBoard(roundWiseScore);
        }
    }
}