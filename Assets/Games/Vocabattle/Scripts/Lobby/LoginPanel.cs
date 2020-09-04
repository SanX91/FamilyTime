using System;
using Game.General;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Vocabattle.Lobby
{
    public class LoginPanel : UIPanel
    {
        public EventHandler<string> OnLoginClickedEvent;

        public InputField teamNameInputField;

        public void OnLoginClicked()
        {
            //TODO - Add checks and error messages for wrong input
            OnLoginClickedEvent?.Invoke(this, teamNameInputField.text);
        }
    }
}