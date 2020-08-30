using System;
using Game.General;
using UnityEngine.UI;

namespace Game.Vocabattle.Lobby
{
    public class CreateGamePanel : UIPanel
    {
        public EventHandler<string> OnCreateGameClickedEvent;
        public EventHandler<EventArgs> OnCancelClickedEvent;
        public InputField gameNameInputField;

        public void OnCreateGameClicked()
        {
            //TODO - Add checks and error messages for wrong input
            OnCreateGameClickedEvent?.Invoke(this, gameNameInputField.text);
        }

        public void OnCancelClicked()
        {
            OnCancelClickedEvent?.Invoke(this, new EventArgs());
        }
    }
}

