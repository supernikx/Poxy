using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public abstract class UIMenu_Base : MonoBehaviour
    {
        [Header("General Settings")]
        [SerializeField]
        protected GameObject defaultSelection;

        /// <summary>
        /// Riferimento all'ui manager
        /// </summary>
        protected UI_ManagerBase uiManager;

        public virtual void Setup(UI_ManagerBase _uiManager)
        {
            uiManager = _uiManager;
        }

        public virtual void Enable()
        {
            gameObject.SetActive(true);
        }

        public virtual void Disable()
        {
            gameObject.SetActive(false);
        }

        public GameObject GetPanelDefaultSelection()
        {
            return defaultSelection;
        }
    }
}
