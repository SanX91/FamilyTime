using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;

namespace Game.Vocabattle.Game
{
    public class GameSetup : MonoBehaviourPunCallbacks
    {
        public int rounds = 5;
        public float turnTime = 60;
        private Coroutine gameRoutine;
        private double currentTurnStartTime;
        private TurnTimeLeftEvent turnTimeLeftEvent;
        private const double NetworkTimeMaxValue = 4294967.295;

        public override void OnEnable()
        {
            base.OnEnable();
            PlayerNumbering.OnPlayerNumberingChanged += OnPlayerNumberingChanged;
            PhotonNetwork.AddCallbackTarget(this);
        }

        public override void OnDisable()
        {
            base.OnDisable();
            PlayerNumbering.OnPlayerNumberingChanged -= OnPlayerNumberingChanged;
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        private void Start()
        {
            turnTimeLeftEvent = new TurnTimeLeftEvent();

            Hashtable props = new Hashtable
            {
                {VocabattleConstants.HasPlayerLoadedLevel, true}
            };

            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }

        private void Update()
        {
            if (turnTime - GetNetworkTimeElapsedSinceTurnStart() >= 0)
            {
                turnTimeLeftEvent.SetValue(turnTime - GetNetworkTimeElapsedSinceTurnStart());
                EventManager.Instance.TriggerEvent(turnTimeLeftEvent);
            }
        }

        private double GetNetworkTimeElapsedSinceTurnStart()
        {
            if (PhotonNetwork.Time - currentTurnStartTime < 0)
            {
                return NetworkTimeMaxValue - currentTurnStartTime + PhotonNetwork.Time;
            }

            return PhotonNetwork.Time - currentTurnStartTime;
        }

        #region PUN CALLBACKS

        public void OnEvent(EventData photonEvent)
        {

        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Vocabattle_lobby");
        }

        public override void OnLeftRoom()
        {
            PhotonNetwork.Disconnect();
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {

        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            // CheckEndOfGame();
        }

        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            if (propertiesThatChanged.ContainsKey(VocabattleConstants.CurrentRound))
            {
                Debug.Log($"Current round is: {GetCurrentRound()}");
                EventManager.Instance.TriggerEvent(new RoundUpdateEvent(GetCurrentRound()));
            }

            if (propertiesThatChanged.ContainsKey(VocabattleConstants.CurrentPlayerTurn))
            {
                Debug.Log($"Current player with turn is: {GetCurrentPlayerWithTurn()}");
                bool isLocal = PhotonNetwork.LocalPlayer.UserId.Equals(GetCurrentPlayerWithTurn());
                PlayerTurnEvent.PlayerTurnData playerTurnData = new PlayerTurnEvent.PlayerTurnData(GetCurrentPlayerWithTurn(), isLocal);
                EventManager.Instance.TriggerEvent(new PlayerTurnEvent(playerTurnData));
            }

            if (propertiesThatChanged.ContainsKey(VocabattleConstants.CurrentTurnStartTime))
            {
                Debug.Log($"Current turn start time is: {GetCurrentTurnStartTime()}");
                currentTurnStartTime = GetCurrentTurnStartTime();
            }
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            // if there was no countdown yet, the master client (this one) waits until everyone loaded the level and sets a timer start
            // int startTimestamp;
            // bool startTimeIsSet = CountdownTimer.TryGetStartTime(out startTimestamp);

            if (changedProps.ContainsKey(VocabattleConstants.HasPlayerLoadedLevel))
            {
                if (CheckAllPlayerLoadedLevel())
                {
                    // if (!startTimeIsSet)
                    // {
                    //     CountdownTimer.SetStartTime();
                    // }

                    StartGame();
                }
                else
                {
                    // not all players loaded yet. wait:
                    // Debug.Log("setting text waiting for players! ",this.InfoText);
                    // InfoText.text = "Waiting for other players...";
                }
            }

        }

        #endregion

        private bool CheckAllPlayerLoadedLevel()
        {
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                object playerLoadedLevel;

                if (p.CustomProperties.TryGetValue(VocabattleConstants.HasPlayerLoadedLevel, out playerLoadedLevel))
                {
                    if ((bool)playerLoadedLevel)
                    {
                        continue;
                    }
                }

                return false;
            }

            return true;
        }

        private void OnPlayerNumberingChanged()
        {
        }

        private void StartGame()
        {
            gameRoutine = StartCoroutine(GameRoutine());
        }

        private System.Collections.IEnumerator GameRoutine()
        {
            int round = GetCurrentRound();
            SetRoomProperty(VocabattleConstants.CurrentRound, round);

            Player[] players = PhotonNetwork.PlayerList;

            while (round <= rounds)
            {
                foreach (Player p in players)
                {
                    SetRoomProperty(VocabattleConstants.CurrentPlayerTurn, p.UserId);
                    SetRoomProperty(VocabattleConstants.CurrentTurnStartTime, PhotonNetwork.Time);
                    yield return new WaitForSeconds(turnTime);
                }

                round++;
                SetRoomProperty(VocabattleConstants.CurrentRound, round);
            }
        }

        private void SetRoomProperty(object key, object value)
        {
            Hashtable props = new Hashtable() { { key, value } };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }

        private int GetCurrentRound()
        {
            object round;

            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(VocabattleConstants.CurrentRound, out round))
            {
                return (int)round;
            }

            return 1;
        }

        private string GetCurrentPlayerWithTurn()
        {
            object userId;

            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(VocabattleConstants.CurrentPlayerTurn, out userId))
            {
                return (string)userId;
            }

            return string.Empty;
        }

        private double GetCurrentTurnStartTime()
        {
            object turnStartTime;

            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(VocabattleConstants.CurrentTurnStartTime, out turnStartTime))
            {
                return (double)turnStartTime;
            }

            return -1;
        }
    }

}