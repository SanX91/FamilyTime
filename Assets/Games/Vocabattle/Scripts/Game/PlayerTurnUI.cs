using UnityEngine;
using UnityEngine.UI;

namespace Game.Vocabattle.Game
{
    public class PlayerTurnUI : MonoBehaviour
    {
        public Text infoText;
        public InputField wordInputField;
        public Text errorText;
        private const string InfoPrefix = "Create a word with the letter";

        public void Show(string letter)
        {
            infoText.text = $"{InfoPrefix} \"{letter}\"";
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void OnSubmit()
        {

        }
    }
}