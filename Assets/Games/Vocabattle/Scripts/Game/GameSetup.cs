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

        public override void OnEnable()
        {
            base.OnEnable();
            PlayerNumbering.OnPlayerNumberingChanged += OnPlayerNumberingChanged;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            PlayerNumbering.OnPlayerNumberingChanged -= OnPlayerNumberingChanged;
        }

        private void Start()
        {
            Hashtable props = new Hashtable
            {
                {VocabattleConstants.HasPlayerLoadedLevel, true}
            };

            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }

        #region PUN CALLBACKS

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
            if (PhotonNetwork.IsMasterClient)
            {
                return;
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
            throw new NotImplementedException();
        }

        // private void StartGame()
        // {
        //     if (PhotonNetwork.IsMasterClient)
        //     {
        //         gameRoutine = StartCoroutine(GameRoutine());
        //     }
        // }

        // private System.Collections.IEnumerator GameRoutine()
        // {
        //     int round = 1;
        //     Player[] players = PhotonNetwork.PlayerList;

        //     while (round < rounds)
        //     {
        //         foreach (Player p in players)
        //         {
        //             Hashtable props = new Hashtable() { { VocabattleConstants.CurrentPlayerTurn, p. } };
        //             PhotonNetwork.CurrentRoom.set()
        //         }

        //         round++;
        //     }
        // }
    }

}