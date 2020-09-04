using System;
using System.Collections.Generic;
using Game.General;
using Photon.Realtime;
using UnityEngine;

namespace Game.Vocabattle.Lobby
{
    public class AvailableGamesPanel : UIPanel
    {
        public EventHandler<EventArgs> OnBackClickedEvent;

        public AvailableGameUI availableGamePrefab;
        public RectTransform availableGamesContainer;
        private Dictionary<string, AvailableGameUI> availableGames;

        private void Awake()
        {
            availableGames = new Dictionary<string, AvailableGameUI>();
        }

        private void OnDisable()
        {
            Reset();
        }

        public void ShowAvailableGames(List<RoomInfo> availableGames, Action<string> onJoinGame)
        {
            foreach (var game in availableGames)
            {
                if (!IsGameValid(game))
                {
                    CheckRemoveGame(game.Name);
                    continue;
                }

                if (this.availableGames.ContainsKey(game.Name))
                {
                    UpdateGame(game);
                    continue;
                }

                CreateGame(game, onJoinGame);
            }
        }

        public void OnBackClicked()
        {
            OnBackClickedEvent?.Invoke(this, new EventArgs());
        }

        private void CreateGame(RoomInfo info, Action<string> onJoinGame)
        {
            AvailableGameUI availableGame = Instantiate(availableGamePrefab, availableGamesContainer);
            availableGame.Initialize(info.Name, onJoinGame);
            availableGame.SetPlayers(info.PlayerCount, info.MaxPlayers);
            availableGames.Add(info.Name, availableGame);
        }

        private bool IsGameValid(RoomInfo info)
        {
            return info.IsOpen && info.IsVisible && !info.RemovedFromList;
        }

        private void CheckRemoveGame(string gameName)
        {
            if (availableGames.ContainsKey(gameName))
            {
                //TODO - Use pooling later
                Destroy(availableGames[gameName].gameObject);
                availableGames.Remove(gameName);
            }
        }

        private void UpdateGame(RoomInfo info)
        {
            availableGames[info.Name].SetPlayers(info.PlayerCount, info.MaxPlayers);
        }

        private void Reset()
        {
            foreach (var game in availableGames)
            {
                Destroy(game.Value.gameObject);
            }

            availableGames = new Dictionary<string, AvailableGameUI>();
        }
    }
}
