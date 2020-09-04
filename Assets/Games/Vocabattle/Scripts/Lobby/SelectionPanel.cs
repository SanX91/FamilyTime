using System;
using Game.General;

namespace Game.Vocabattle.Lobby
{
    public class SelectionPanel : UIPanel
    {
        public EventHandler<EventArgs> OnCreateGameClickedEvent;
        public EventHandler<EventArgs> OnJoinGameClickedEvent;

        public void OnCreateGameClicked()
        {
            OnCreateGameClickedEvent?.Invoke(this, new EventArgs());
        }

        public void OnJoinGameClicked()
        {
            OnJoinGameClickedEvent?.Invoke(this, new EventArgs());
        }
    }
}