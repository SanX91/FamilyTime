using UnityEngine;
using UnityEngine.UI;

namespace Game.Vocabattle.Game
{
    public class OpponentTurnUI : MonoBehaviour
    {
        public Text infoText;

        public void Show(string info)
        {
            infoText.text = info;
            gameObject.SetActive(true);
        }

        public void UpdateInfo(string info)
        {
            infoText.text = info;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            infoText.text = string.Empty;
        }
    }
}