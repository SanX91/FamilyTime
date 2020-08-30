using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Vocabattle.Lobby
{
    public class AvailableGameUI : MonoBehaviour
    {
        public Text gameNameText;
        public Text gamePlayersText;
        private string gameName;
        private Action<string> onJoinGame;

        public void Initialize(string gameName, Action<string> onJoinGame)
        {
            this.gameName = gameName;
            gameNameText.text = gameName;
            this.onJoinGame = onJoinGame;
        }

        public void SetPlayers(int currentPlayers, int maxPlayers)
        {
            gamePlayersText.text = currentPlayers.ToString();
        }

        public void OnJoinGameClicked()
        {
            onJoinGame?.Invoke(gameName);
        }
    }
}