using UnityEngine;
using System.Collections;

namespace UI
{
    public class UIMenu_TutorialPanel : UIMenu_Base
    {
        [Header("Tutorial Sections")]
        [SerializeField]
        private GameObject SectionA;
        [SerializeField]
        private GameObject SectionB;

        UI_ManagerBase uiManager;

        #region API
        public override void Setup(UI_ManagerBase _uiManager)
        {
            uiManager = _uiManager;
        }

        public override void Enable()
        {
            SectionA.SetActive(true);
            SectionB.SetActive(false);

            //events
            PlayerInputManager.OnConfirmPressed += HandleConfirmPressed;

            base.Enable();
            Time.timeScale = 0f;
        }

        public override void Disable()
        {
            Time.timeScale = 1f;

            //events
            PlayerInputManager.OnConfirmPressed -= HandleConfirmPressed;

            base.Disable();
        }
        #endregion

        #region Handlers
        private void HandleConfirmPressed()
        {
            if (!SectionB.activeInHierarchy)
            {
                SectionB.SetActive(true);
            }
            else if (SectionA.activeInHierarchy && SectionB.activeInHierarchy)
            {
                uiManager.ToggleMenu(MenuType.Game);
            }
        }
        #endregion
    }
}

