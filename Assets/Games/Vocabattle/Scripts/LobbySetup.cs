using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;

namespace Game.Vocabattle.Lobby
{
    public class LobbySetup : MonoBehaviourPunCallbacks
    {
        public LobbyUI lobbyUI;
        public string gameSceneName;

        #region UNITY

        private void Awake()
        {
            PhotonNetwork.AutomaticallySyncScene = true;

            lobbyUI.GetPanel<LoginPanel>().OnLoginClickedEvent += OnLoginAttempt;

            lobbyUI.GetPanel<SelectionPanel>().OnCreateGameClickedEvent += OnCreateGamePanelOpen;
            lobbyUI.GetPanel<SelectionPanel>().OnJoinGameClickedEvent += OnJoinGamePanelOpen;

            lobbyUI.GetPanel<CreateGamePanel>().OnCreateGameClickedEvent += OnCreateGameAttempt;
            lobbyUI.GetPanel<CreateGamePanel>().OnCancelClickedEvent += OnCancelCreateGame;

            lobbyUI.GetPanel<AvailableGamesPanel>().OnBackClickedEvent += OnExitAvailableGames;

            lobbyUI.GetPanel<JoinedGamePanel>().OnStartGameClickedEvent += OnStartGameButtonClicked;
            lobbyUI.GetPanel<JoinedGamePanel>().OnLeaveGameClickedEvent += OnLeaveGameButtonClicked;
        }

        private void Start()
        {
            lobbyUI.ActivatePanel<LoginPanel>();
        }

        #endregion

        #region PUN CALLBACKS

        public override void OnConnectedToMaster()
        {
            lobbyUI.ActivatePanel<SelectionPanel>();
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            lobbyUI.GetPanel<AvailableGamesPanel>().ShowAvailableGames(roomList, OnJoinGameClicked);
        }

        public override void OnLeftLobby()
        {

        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            lobbyUI.ActivatePanel<SelectionPanel>();
        }

        public override void OnJoinedRoom()
        {
            lobbyUI.ActivatePanel<JoinedGamePanel>();

            Hashtable initialProps = new Hashtable() { { VocabattleConstants.IsPlayerReady, false } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(initialProps);
            PhotonNetwork.LocalPlayer.SetScore(0);

            lobbyUI.GetPanel<JoinedGamePanel>().ShowJoinedPlayers(PhotonNetwork.PlayerList.ToList(), OnPlayerReady);
            ToggleStartButton();

            Hashtable props = new Hashtable
            {
                {AsteroidsGame.PLAYER_LOADED_LEVEL, false}
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }

        public override void OnLeftRoom()
        {
            lobbyUI.ActivatePanel<SelectionPanel>();
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            lobbyUI.GetPanel<JoinedGamePanel>().AddPlayer(newPlayer, OnPlayerReady);
            ToggleStartButton();
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            lobbyUI.GetPanel<JoinedGamePanel>().RemovePlayer(otherPlayer);
            ToggleStartButton();
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            ToggleStartButton();
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            lobbyUI.GetPanel<JoinedGamePanel>().UpdatePlayer(targetPlayer, changedProps, OnPlayerReady);
            ToggleStartButton();
        }

        #endregion

        #region UI CALLBACKS

        private void OnLoginAttempt(object sender, string teamName)
        {
            if (string.IsNullOrEmpty(teamName))
            {
                Debug.LogError("Team Name is invalid.");
                return;
            }

            PhotonNetwork.LocalPlayer.NickName = teamName;
            PhotonNetwork.ConnectUsingSettings();
        }

        private void OnCreateGamePanelOpen(object sender, System.EventArgs e)
        {
            lobbyUI.ActivatePanel<CreateGamePanel>();
        }

        private void OnJoinGamePanelOpen(object sender, System.EventArgs e)
        {
            if (!PhotonNetwork.InLobby)
            {
                PhotonNetwork.JoinLobby();
            }

            lobbyUI.ActivatePanel<AvailableGamesPanel>();
        }

        private void OnCreateGameAttempt(object sender, string gameName)
        {
            RoomOptions options = new RoomOptions { PlayerTtl = 10000 };
            PhotonNetwork.CreateRoom(gameName, options, null);
        }

        private void OnCancelCreateGame(object sender, System.EventArgs e)
        {
            lobbyUI.ActivatePanel<SelectionPanel>();
        }

        private void OnJoinGameClicked(string gameName)
        {
            if (PhotonNetwork.InLobby)
            {
                PhotonNetwork.LeaveLobby();
            }

            PhotonNetwork.JoinRoom(gameName);
        }

        private void OnPlayerReady(bool isReady)
        {
            Hashtable props = new Hashtable() { { VocabattleConstants.IsPlayerReady, isReady } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }

        private void OnExitAvailableGames(object sender, System.EventArgs e)
        {
            if (PhotonNetwork.InLobby)
            {
                PhotonNetwork.LeaveLobby();
            }

            lobbyUI.ActivatePanel<SelectionPanel>();
        }

        public void OnLeaveGameButtonClicked(object sender, System.EventArgs e)
        {
            PhotonNetwork.LeaveRoom();
        }

        public void OnStartGameButtonClicked(object sender, System.EventArgs e)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;

            PhotonNetwork.LoadLevel(gameSceneName);
        }

        #endregion

        private bool AreAllPlayersReady()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return false;
            }

            foreach (Player p in PhotonNetwork.PlayerList)
            {
                object isPlayerReady;
                if (p.CustomProperties.TryGetValue(VocabattleConstants.IsPlayerReady, out isPlayerReady))
                {
                    if (!(bool)isPlayerReady)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        private void ToggleStartButton()
        {
            lobbyUI.GetPanel<JoinedGamePanel>().ToggleStartButton(AreAllPlayersReady());
        }

        private void OnApplicationQuit()
        {
            MonoBehaviour[] scripts = Object.FindObjectsOfType<MonoBehaviour>();
            foreach (MonoBehaviour script in scripts)
            {
                script.enabled = false;
            }
        }
    }
}
