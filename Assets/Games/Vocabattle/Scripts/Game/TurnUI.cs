using System;
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

        private void OnEnable()
        {
            EventManager.Instance.AddListener<RoundUpdateEvent>(OnRoundUpdate);
            EventManager.Instance.AddListener<PlayerTurnEvent>(OnPlayerTurn);
            EventManager.Instance.AddListener<TurnTimeLeftEvent>(OnTurnTimeLeft);
        }

        private void OnDisable()
        {
            EventManager.Instance.RemoveListener<RoundUpdateEvent>(OnRoundUpdate);
            EventManager.Instance.RemoveListener<PlayerTurnEvent>(OnPlayerTurn);
            EventManager.Instance.RemoveListener<TurnTimeLeftEvent>(OnTurnTimeLeft);
        }

        private void OnTurnTimeLeft(TurnTimeLeftEvent evt)
        {
            double timeLeft = (double)evt.GetData();
            turnTimerText.text = $"Time Left: {timeLeft.ToString("F0")}";
        }

        private void OnPlayerTurn(PlayerTurnEvent evt)
        {
            PlayerTurnEvent.PlayerTurnData playerTurnData = (PlayerTurnEvent.PlayerTurnData)evt.GetData();
            string turnMessage = playerTurnData.IsLocal ? "Your turn!" : $"{playerTurnData.CurrentPlayer}'s turn!";
            turnMessageText.text = turnMessage;

            if (playerTurnData.IsLocal)
            {
                opponentTurnUI.Hide();
                playerTurnUI.Show("g");
                return;
            }

            playerTurnUI.Hide();
            opponentTurnUI.Show("Waiting for opponent to submit a word with b");
        }

        private void OnRoundUpdate(RoundUpdateEvent evt)
        {
            int round = (int)evt.GetData();
            roundText.text = $"Round: {round}";
        }
    }
}