using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class UIMenu_LevelSelection : UIMenu_Base
    {
        #region Actions
        public static Action<LevelScriptable> OnLevelSelected;
        #endregion

        public override void Setup(UI_ManagerBase uiManager)
        {
            base.Setup(uiManager);
        }

        public override void Enable()
        {
            base.Enable();
            OnLevelSelected += HandleLevelSelected;
        }

        private void HandleLevelSelected(LevelScriptable _levelSelected)
        {
            Debug.Log(_levelSelected.LevelName);
        }

        public override void Disable()
        {
            base.Disable();
            OnLevelSelected -= HandleLevelSelected;
        }
    }
}
