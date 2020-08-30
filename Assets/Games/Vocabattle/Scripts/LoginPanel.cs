using System;
using Game.General;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Vocabattle.Lobby
{
    public class LoginPanel : UIPanel
    {
        public EventHandler<string> OnLoginClickedEvent;

        public InputField teamNameInput;

        public void OnLoginClicked()
        {
            OnLoginClickedEvent?.Invoke(this, teamNameInput.text);
        }
    }
}