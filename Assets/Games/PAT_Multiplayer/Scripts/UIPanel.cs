using UnityEngine;

namespace Game.General
{
    public abstract class UIPanel : MonoBehaviour
    {
        public virtual void Toggle(bool isActive)
        {
            gameObject.SetActive(isActive);
        }
    }
}