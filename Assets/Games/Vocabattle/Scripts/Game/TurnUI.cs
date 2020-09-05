using System;
using System.Collections.Generic;
using Game.General;
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
            EventManager.Instance.AddListener<WordsLoadedEvent>(OnWordsLoaded);
            EventManager.Instance.AddListener<UsedWordsEvent>(OnUsedWords);
        }

        private void OnDisable()
        {
            EventManager.Instance.RemoveListener<RoundUpdateEvent>(OnRoundUpdate);
            EventManager.Instance.RemoveListener<PlayerTurnEvent>(OnPlayerTurn);
            EventManager.Instance.RemoveListener<TurnTimeLeftEvent>(OnTurnTimeLeft);
            EventManager.Instance.RemoveListener<WordsLoadedEvent>(OnWordsLoaded);
            EventManager.Instance.RemoveListener<UsedWordsEvent>(OnUsedWords);
        }

        private void OnUsedWords(UsedWordsEvent evt)
        {
            List<string> usedWords = (List<string>)evt.GetData();
            playerTurnUI.SetUsedWords(usedWords);
        }

        private void OnWordsLoaded(WordsLoadedEvent evt)
        {
            Dictionary<string, object> words = (Dictionary<string, object>)evt.GetData();
            playerTurnUI.SetWords(words);
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
                playerTurnUI.Show(playerTurnData.TargetLetter);
                return;
            }

            playerTurnUI.Hide();
            opponentTurnUI.Show($"Waiting for opponent to submit a word with {playerTurnData.TargetLetter}");
        }

        private void OnRoundUpdate(RoundUpdateEvent evt)
        {
            int round = (int)evt.GetData();
            roundText.text = $"Round: {round}";
        }
    }
}