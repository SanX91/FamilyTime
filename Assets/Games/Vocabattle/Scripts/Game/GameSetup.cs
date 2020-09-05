using System;
using System.Collections.Generic;
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
        private bool isTurnOver;
        private TurnTimeLeftEvent turnTimeLeftEvent;
        private string targetLetter;
        private bool isGameOver;
        private List<string> usedWords;
        private const double NetworkTimeMaxValue = 4294967.295;

        public override void OnEnable()
        {
            base.OnEnable();
            EventManager.Instance.AddListener<SubmitWordEvent>(OnSubmitWord);
        }

        public override void OnDisable()
        {
            base.OnDisable();
            EventManager.Instance.RemoveListener<SubmitWordEvent>(OnSubmitWord);
        }

        private void OnSubmitWord(SubmitWordEvent evt)
        {
            string word = (string)evt.GetData();
            string round = $"round_{GetCurrentRound()}";

            if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey(round))
            {
                return;
            }

            SetPlayerProperty(round, word);
            PhotonNetwork.LocalPlayer.AddScore(word.Length);
        }

        private void Start()
        {
            turnTimeLeftEvent = new TurnTimeLeftEvent();
            usedWords = new List<string>();
            EventManager.Instance.TriggerEvent(new UsedWordsEvent(usedWords));

            Hashtable props = new Hashtable
            {
                {VocabattleConstants.HasPlayerLoadedLevel, true}
            };

            PhotonNetwork.LocalPlayer.SetCustomProperties(props);

            EventManager.Instance.TriggerEvent(new CreateScoreBoardsEvent(PhotonNetwork.PlayerList));
        }

        private void Update()
        {
            if (isGameOver)
            {
                return;
            }

            if (turnTime - GetNetworkTimeElapsedSinceTurnStart() >= 0)
            {
                turnTimeLeftEvent.SetValue(turnTime - GetNetworkTimeElapsedSinceTurnStart());
                EventManager.Instance.TriggerEvent(turnTimeLeftEvent);
                return;
            }
        }

        private bool IsTimeUp()
        {
            if (turnTime - GetNetworkTimeElapsedSinceTurnStart() >= 0)
            {
                return false;
            }

            isTurnOver = true;
            SetRoomProperty(VocabattleConstants.IsTurnOver, true);
            return true;
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
            if (gameRoutine != null)
            {
                StopCoroutine(gameRoutine);
                gameRoutine = null;
            }

            if (PhotonNetwork.LocalPlayer.UserId != newMasterClient.UserId)
            {
                return;
            }

            gameRoutine = StartCoroutine(GameRoutine());
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
                PlayerTurnEvent.PlayerTurnData playerTurnData = new PlayerTurnEvent.PlayerTurnData(GetCurrentPlayerWithTurn(), isLocal, targetLetter);
                EventManager.Instance.TriggerEvent(new PlayerTurnEvent(playerTurnData));
            }

            if (propertiesThatChanged.ContainsKey(VocabattleConstants.CurrentTurnStartTime))
            {
                Debug.Log($"Current turn start time is: {GetCurrentTurnStartTime()}");
                currentTurnStartTime = GetCurrentTurnStartTime();
            }

            if (propertiesThatChanged.ContainsKey(VocabattleConstants.TargetLetter))
            {
                Debug.Log($"Current turn target letter is: {GetTargetLetter()}");
                targetLetter = GetTargetLetter();
            }

            if (propertiesThatChanged.ContainsKey(VocabattleConstants.IsGameOver))
            {
                Debug.Log($"Is game over: {IsGameOver()}");
                isGameOver = IsGameOver();

                if (isGameOver)
                {
                    GameOverEvent.RoundWiseScore roundWiseScore = new GameOverEvent.RoundWiseScore(PhotonNetwork.PlayerList, rounds);
                    EventManager.Instance.TriggerEvent(new GameOverEvent(roundWiseScore));
                }
            }
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (changedProps.ContainsKey(PunPlayerScores.PlayerScoreProp))
            {
                ScoreUpdateEvent.RoundWiseScore roundWiseScore = new ScoreUpdateEvent.RoundWiseScore(targetPlayer, rounds);
                EventManager.Instance.TriggerEvent(new ScoreUpdateEvent(roundWiseScore));
            }

            CheckWord(targetPlayer, changedProps);

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
                    StartGame();
                }
            }
        }

        private void CheckWord(Player targetPlayer, Hashtable changedProps)
        {
            string key = $"{VocabattleConstants.Round}_{GetCurrentRound()}";
            if (!changedProps.ContainsKey(key))
            {
                return;
            }

            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log($"Receiving player update!");
                isTurnOver = true;
                SetRoomProperty(VocabattleConstants.IsTurnOver, true);
            }

            string word = GetWord(targetPlayer, key);
            if (string.IsNullOrEmpty(word))
            {
                return;
            }

            usedWords.Add(word);

            if (PhotonNetwork.IsMasterClient)
            {
                SetRoomProperty(VocabattleConstants.TargetLetter, word[word.Length - 1].ToString());
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

        private void StartGame()
        {
            gameRoutine = StartCoroutine(GameRoutine());
        }

        private System.Collections.IEnumerator GameRoutine()
        {
            int round = GetCurrentRound();
            SetRoomProperty(VocabattleConstants.CurrentRound, round);

            Player[] players = PhotonNetwork.PlayerList;
            string currentPlayerWithTurn = GetCurrentPlayerWithTurn();
            Player currentPlayer = null;

            if (!string.IsNullOrEmpty(currentPlayerWithTurn))
            {
                currentPlayer = FindPlayerWithUserId(currentPlayerWithTurn);
            }
            else if (players != null && players.Length > 0)
            {
                currentPlayer = players[0];
            }

            targetLetter = GetTargetLetter();

            if (string.IsNullOrEmpty(targetLetter))
            {
                int random = UnityEngine.Random.Range(0, 26);
                char alphabet = (char)('a' + random);
                SetRoomProperty(VocabattleConstants.TargetLetter, alphabet.ToString());
            }

            //TODO - Load proper time on master client switch
            while (round <= rounds)
            {
                Debug.Log($"Player number: {currentPlayer.GetPlayerNumber()}");
                for (int i = currentPlayer.GetPlayerNumber(); i < players.Length; i++)
                {
                    isTurnOver = false;
                    SetRoomProperty(VocabattleConstants.IsTurnOver, false);
                    SetRoomProperty(VocabattleConstants.CurrentPlayerTurn, currentPlayer.UserId);
                    currentTurnStartTime = PhotonNetwork.Time;
                    SetRoomProperty(VocabattleConstants.CurrentTurnStartTime, currentTurnStartTime);
                    yield return new WaitUntil(() => isTurnOver || IsTimeUp());
                    currentPlayer = currentPlayer.GetNext();
                }

                if (round >= rounds)
                {
                    SetRoomProperty(VocabattleConstants.IsGameOver, true);
                    break;
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

        private void SetPlayerProperty(object key, object value)
        {
            Hashtable props = new Hashtable
            {
                {key, value}
            };

            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
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

        private bool IsTurnOver()
        {
            object isTurnOver;

            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(VocabattleConstants.IsTurnOver, out isTurnOver))
            {
                return (bool)isTurnOver;
            }

            return false;
        }

        private Player FindPlayerWithUserId(string userId)
        {
            foreach (var player in PhotonNetwork.PlayerList)
            {
                if (player.UserId.Equals(userId))
                {
                    return player;
                }
            }

            return null;
        }

        private string GetWord(Player player, string key)
        {
            object word;

            if (player.CustomProperties.TryGetValue(key, out word))
            {
                return (string)word;
            }

            return string.Empty;
        }

        private string GetTargetLetter()
        {
            object targetLetter;

            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(VocabattleConstants.TargetLetter, out targetLetter))
            {
                return (string)targetLetter;
            }

            return string.Empty;
        }

        private bool IsGameOver()
        {
            object isGameOver;

            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(VocabattleConstants.IsGameOver, out isGameOver))
            {
                return (bool)isGameOver;
            }

            return false;
        }
    }
}