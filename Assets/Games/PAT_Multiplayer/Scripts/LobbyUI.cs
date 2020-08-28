using System.Collections.Generic;
using Game.General;
using UnityEngine;

namespace Game.PAT_Multiplayer.Lobby
{
    public class LobbyUI : MonoBehaviour
    {
        public List<UIPanel> uiPanels;

        public T GetPanel<T>() where T : UIPanel
        {
            return (T)uiPanels.Find(x => x.GetType() == typeof(T));
        }

        public void ActivatePanel<T>() where T : UIPanel
        {
            foreach (var uiPanel in uiPanels)
            {
                if (uiPanel.GetType() == typeof(T))
                {
                    uiPanel.Toggle(true);
                    continue;
                }

                uiPanel.Toggle(false);
            }
        }
    }
}

