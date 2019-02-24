using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public abstract class UIMenu_Base : MonoBehaviour
    {
        public abstract void Setup(UI_ManagerBase uiManager);

        public virtual void Enable()
        {
            gameObject.SetActive(true);
        }

        public virtual void Disable()
        {
            gameObject.SetActive(false);
        }
    }
}
