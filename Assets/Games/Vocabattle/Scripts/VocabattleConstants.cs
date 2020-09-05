using UnityEngine;

namespace Game.Vocabattle
{
    public static class VocabattleConstants
    {
        public const string IsPlayerReady = "isPlayerReady";
        public const string HasPlayerLoadedLevel = "hasPlayerLoadedLevel";
        public const string GameTimeElapsed = "gameTimeElapsed";
        public const string CurrentTurnStartTime = "currentTurnStartTime";
        public const string CurrentPlayerTurn = "currentPlayerTurn";
        public const string CurrentRound = "currentRound";
        public const string IsTurnOver = "isTurnOver";
        public const string TeamNamePref = "teamNamePref";
        public const string GameNamePref = "gameNamePref";
        public const string Round = "round";
        public const string TargetLetter = "targetLetter";
        public const string IsGameOver = "isGameOver";
        
        public static Color GetColor(int number)
        {
            switch (number)
            {
                case 0: return Color.red;
                case 1: return Color.green;
                case 2: return Color.blue;
                case 3: return Color.yellow;
                case 4: return Color.cyan;
                case 5: return Color.grey;
                case 6: return Color.magenta;
                case 7: return Color.white;
            }

            return Color.black;
        }
    }
}