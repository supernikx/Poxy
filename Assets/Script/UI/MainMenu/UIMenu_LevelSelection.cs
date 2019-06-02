using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class UIMenu_LevelSelection : UIMenu_Base
    {
        #region Actions
        public Action<UI_SelectLevelButton> OnLevelButtonPressed;
        public Action OnLevelButtonDeselected;
        public Action<bool> OnModeButtonPressed;
        public static Action<LevelScriptable, bool> OnLevelSelected;
        #endregion

        List<UI_SelectLevelButton> levelsButtons = new List<UI_SelectLevelButton>();
        private UI_SelectLevelButton levelSelected;
        private bool selectingMode;

        public override void Enable()
        {
            base.Enable();

            OnModeButtonPressed += HandleModeSelected;
            OnLevelButtonPressed += HandleLevelSelected;
            OnLevelButtonDeselected += HandleLevelDeSelected;

            levelSelected = null;
            selectingMode = false;
            EnableAllButtons(true);

            SetupLevelsButtons();
        }

        private void HandleLevelSelected(UI_SelectLevelButton _levelSelected)
        {
            levelSelected = _levelSelected;
            LevelScriptable levelScriptable = _levelSelected.GetLevelScriptable();
            if (levelScriptable.TutorialLevel)
            {
                if (OnLevelSelected != null)
                    OnLevelSelected(levelSelected.GetLevelScriptable(), false);
            }
            else
            {
                EnableAllButtons(false);
                selectingMode = true;
                levelSelected.SelectModePanel(true);
            }
        }

        private void HandleLevelDeSelected()
        {
            levelSelected.SelectModePanel(false);
            levelSelected = null;
            selectingMode = false;
            EnableAllButtons(true);
        }

        private void HandleModeSelected(bool _speedRun)
        {
            if (levelSelected != null && selectingMode)
            {
                if (OnLevelSelected != null)
                    OnLevelSelected(levelSelected.GetLevelScriptable(), _speedRun);
            }
        }

        public void EnableAllButtons(bool _enable)
        {
            foreach (UI_SelectLevelButton lvlButton in levelsButtons)
            {
                lvlButton.EnableButton(_enable);
            }
        }

        public GameObject GetButtonToSelect()
        {
            if (selectingMode && levelSelected != null)
                return levelSelected.GetModeSelectionPanel().GetDefaultButton();
            else
                return defaultSelection;
        }

        public void BackButton()
        {
            if (selectingMode)
                HandleLevelDeSelected();
            else
                uiManager.ToggleMenu(MenuType.MainMenu);
        }

        public override void Disable()
        {
            base.Disable();

            OnModeButtonPressed -= HandleModeSelected;
            OnLevelButtonPressed -= HandleLevelSelected;
            OnLevelButtonDeselected -= HandleLevelDeSelected;
        }

        private void SetupLevelsButtons()
        {
            levelsButtons = GetComponentsInChildren<UI_SelectLevelButton>().ToList();

            for (int i = 0; i < levelsButtons.Count; i++)
            {
                levelsButtons[i].Init(this);
            }
        }
    }
}
