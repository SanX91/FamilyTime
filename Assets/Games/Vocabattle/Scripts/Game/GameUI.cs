using UnityEngine;

namespace Game.Vocabattle.Game
{
    public class GameUI : MonoBehaviour
    {
        public TurnUI turnUI;
        public ScoreUI scoreUI;
        public GameOverPanel gameOverPanel;

        private void OnEnable()
        {
            EventManager.Instance.AddListener<GameOverEvent>(OnGameOver);
        }

        private void OnDisable()
        {
            EventManager.Instance.RemoveListener<GameOverEvent>(OnGameOver);
        }

        private void OnGameOver(GameOverEvent evt)
        {
            GameOverEvent.RoundWiseScore roundWiseScores = (GameOverEvent.RoundWiseScore)evt.GetData();
            gameOverPanel.Initialize(roundWiseScores);
        }
    }
}