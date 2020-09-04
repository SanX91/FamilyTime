using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Game.General;
using Photon.Realtime;
using UnityEngine;

namespace Game.Vocabattle.Lobby
{
    public class JoinedGamePanel : UIPanel
    {
        public EventHandler<EventArgs> OnStartGameClickedEvent;
        public EventHandler<EventArgs> OnLeaveGameClickedEvent;

        public GameObject startButton;
        public JoinedPlayerUI joinedPlayerPrefab;
        public RectTransform joinedPlayersContainer;
        private Dictionary<int, JoinedPlayerUI> joinedPlayers;

        private void Awake()
        {
            joinedPlayers = new Dictionary<int, JoinedPlayerUI>();
        }

        private void OnDisable()
        {
            Reset();
        }

        public void ShowJoinedPlayers(List<Player> joinedPlayers, Action<bool> onReady)
        {
            foreach (var player in joinedPlayers)
            {
                if (this.joinedPlayers.ContainsKey(player.ActorNumber))
                {
                    continue;
                }

                CreatePlayer(player, onReady);
            }
        }

        public void AddPlayer(Player player, Action<bool> onReady)
        {
            if (joinedPlayers.ContainsKey(player.ActorNumber))
            {
                return;
            }

            CreatePlayer(player, onReady);
        }

        public void RemovePlayer(Player player)
        {
            if (!joinedPlayers.ContainsKey(player.ActorNumber))
            {
                return;
            }

            //TODO - Use pooling later
            Destroy(joinedPlayers[player.ActorNumber].gameObject);
            joinedPlayers.Remove(player.ActorNumber);
        }

        public void UpdatePlayer(Player targetPlayer, Hashtable changedProps, Action<bool> onReady)
        {
            if (!joinedPlayers.ContainsKey(targetPlayer.ActorNumber))
            {
                CreatePlayer(targetPlayer, onReady);
                return;
            }

            if (changedProps.ContainsKey(VocabattleConstants.IsPlayerReady))
            {
                SetPlayerReady(targetPlayer);
            }
        }

        public void ToggleStartButton(bool areAllPlayersReady)
        {
            startButton.SetActive(areAllPlayersReady);
        }

        public void OnStartGame()
        {
            OnStartGameClickedEvent?.Invoke(this, new EventArgs());
        }

        public void OnLeaveGame()
        {
            OnLeaveGameClickedEvent?.Invoke(this, new EventArgs());
        }

        private void CreatePlayer(Player player, Action<bool> onReady)
        {
            JoinedPlayerUI joinedPlayer = Instantiate(joinedPlayerPrefab, joinedPlayersContainer);
            joinedPlayer.Initialize(player.ActorNumber, player.NickName, onReady);
            joinedPlayers.Add(player.ActorNumber, joinedPlayer);

            SetPlayerReady(player);
        }

        private void SetPlayerReady(Player player)
        {
            if (!joinedPlayers.ContainsKey(player.ActorNumber))
            {
                return;
            }

            object isPlayerReady;
            if (player.CustomProperties.TryGetValue(VocabattleConstants.IsPlayerReady, out isPlayerReady))
            {
                joinedPlayers[player.ActorNumber].SetPlayerReady((bool)isPlayerReady);
            }
        }

        private void Reset()
        {
            foreach (var player in joinedPlayers)
            {
                Destroy(player.Value.gameObject);
            }

            joinedPlayers = new Dictionary<int, JoinedPlayerUI>();
        }
    }
}
