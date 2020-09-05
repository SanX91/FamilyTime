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

            public PlayerTurnData(string currentPlayer, bool isLocal = false)
            {
                CurrentPlayer = currentPlayer;
                IsLocal = isLocal;
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
}