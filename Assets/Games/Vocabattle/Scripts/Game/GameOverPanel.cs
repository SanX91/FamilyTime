using UnityEngine;
using UnityEngine.UI;

namespace Game.Vocabattle.Game
{
    public class GameOverPanel : MonoBehaviour
    {
        public Text victorTeamNameText;
        public TeamScoreBoardUI scoreboardPrefab;
        public RectTransform scoreBoardContainer;

        public void SetVictorMessageText(string victorTeamName)
        {
            victorTeamNameText.text = victorTeamName;
        }

        public void CreateScoreboards()
        {
            //TODO Create scoreboards
        }
    }
}