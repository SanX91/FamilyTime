using System.Collections.Generic;
using Photon.Realtime;

namespace Game.Vocabattle.Game
{
    public class RoundUpdateEvent : IEvent
    {
        private int round;
        public RoundUpdateEvent(int round)
        {
            this.round = round;
        }
        public object GetData()
        {
            return round;
        }
    }

    public class PlayerTurnEvent : IEvent
    {
        private PlayerTurnData playerTurnData;
        public PlayerTurnEvent(PlayerTurnData playerTurnData)
        {
            this.playerTurnData = playerTurnData;
        }
        public object GetData()
        {
            return playerTurnData;
        }

        public class PlayerTurnData
        {
            public string CurrentPlayer { get; }
            public bool IsLocal { get; }
            public string TargetLetter { get; }

            public PlayerTurnData(string currentPlayer, bool isLocal, string targetLetter)
            {
                CurrentPlayer = currentPlayer;
                IsLocal = isLocal;
                TargetLetter = targetLetter;
            }
        }
    }

    public class TurnTimeLeftEvent : IEvent
    {
        private double turnTimeLeft;
        public void SetValue(double turnTimeLeft)
        {
            this.turnTimeLeft = turnTimeLeft;
        }
        public object GetData()
        {
            return turnTimeLeft;
        }
    }

    public class CreateScoreBoardsEvent : IEvent
    {
        private Player[] players;
        public CreateScoreBoardsEvent(Player[] players)
        {
            this.players = players;
        }
        public object GetData()
        {
            return players;
        }
    }

    public class ScoreUpdateEvent : IEvent
    {
        private RoundWiseScore roundWiseScore;
        public ScoreUpdateEvent(RoundWiseScore roundWiseScore)
        {
            this.roundWiseScore = roundWiseScore;
        }
        public object GetData()
        {
            return roundWiseScore;
        }

        public class RoundWiseScore
        {
            public Player Player { get; }
            public int MaxRounds { get; }

            public RoundWiseScore(Player player, int maxRounds)
            {
                Player = player;
                MaxRounds = maxRounds;
            }
        }
    }

    public class SubmitWordEvent : IEvent
    {
        private string word;
        public SubmitWordEvent(string word)
        {
            this.word = word;
        }
        public object GetData()
        {
            return word;
        }
    }

    public class GameOverEvent : IEvent
    {
        private RoundWiseScore roundWiseScore;
        public GameOverEvent(RoundWiseScore roundWiseScore)
        {
            this.roundWiseScore = roundWiseScore;
        }
        public object GetData()
        {
            return roundWiseScore;
        }

        public class RoundWiseScore
        {
            public Player[] Players { get; }
            public int MaxRounds { get; }

            public RoundWiseScore(Player[] players, int maxRounds)
            {
                Players = players;
                MaxRounds = maxRounds;
            }
        }
    }

    public class UsedWordsEvent : IEvent
    {
        private List<string> usedWords;
        public UsedWordsEvent(List<string> usedWords)
        {
            this.usedWords = usedWords;
        }
        public object GetData()
        {
            return usedWords;
        }
    }
}