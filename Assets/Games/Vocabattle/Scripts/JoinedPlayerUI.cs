using System;
using System.Collections;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Vocabattle.Lobby
{
    public class JoinedPlayerUI : MonoBehaviour
    {
        public Text playerNameText;
        public Image playerColorImage;
        public Button playerReadyButton;
        public Image playerReadyImage;
        public Text playerReadyText;

        private int playerId;
        private string playerName;
        private Action<bool> onPlayerReady;

        private bool isPlayerReady;

        #region UNITY

        private void OnEnable()
        {
            PlayerNumbering.OnPlayerNumberingChanged += OnPlayerNumberingChanged;
        }

        private void OnDisable()
        {
            PlayerNumbering.OnPlayerNumberingChanged -= OnPlayerNumberingChanged;
        }

        #endregion

        public void Initialize(int playerId, string playerName, Action<bool> onPlayerReady)
        {
            this.playerId = playerId;
            this.playerName = playerName;
            playerNameText.text = playerName;

            this.onPlayerReady = onPlayerReady;

            if (PhotonNetwork.LocalPlayer.ActorNumber != playerId)
            {
                playerReadyButton.gameObject.SetActive(false);
            }
        }

        private void OnPlayerNumberingChanged()
        {
            foreach (var player in PhotonNetwork.PlayerList)
            {
                if (player.ActorNumber == playerId)
                {
                    playerColorImage.color = VocabattleConstants.GetColor(player.GetPlayerNumber());
                }
            }
        }

        public void OnPlayerReadyToggle()
        {
            isPlayerReady = !isPlayerReady;
            SetPlayerReady(isPlayerReady);
            onPlayerReady?.Invoke(isPlayerReady);
        }

        public void SetPlayerReady(bool isPlayerReady)
        {
            this.isPlayerReady = isPlayerReady;
            playerReadyText.text = isPlayerReady ? "Ready!" : "Ready?";
            playerReadyImage.enabled = isPlayerReady;
        }
    }
}