using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Vocabattle.Game
{
    public class TurnUI : MonoBehaviour
    {
        public Text turnMessageText;
        public Text roundText;
        public Text turnTimerText;
        public PlayerTurnUI playerTurnUI;
        public OpponentTurnUI opponentTurnUI;
    }
}