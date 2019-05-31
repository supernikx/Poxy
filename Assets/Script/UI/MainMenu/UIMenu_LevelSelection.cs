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
        public static Action<LevelScriptable> OnLevelSelected;
        public static Action<bool> OnModeSelected;
        #endregion

        List<UI_SelectLevelButton> levelsButtons = new List<UI_SelectLevelButton>();

        public override void Setup(UI_ManagerBase uiManager)
        {
            base.Setup(uiManager);

            List<LevelScriptable> levels = Resources.LoadAll<LevelScriptable>("").ToList();
            InstantiateLevelsButtons(levels);
            defaultSelection = levelsButtons[0].gameObject;
        }

        public override void Enable()
        {
            base.Enable();
            OnLevelButtonPressed += HandleLevelSelected;
        }

        private void HandleLevelSelected(UI_SelectLevelButton _levelSelected)
        {
            LevelScriptable levelScriptable = _levelSelected.GetLevelScriptable();
            if (!levelScriptable.TutorialLevel)
                _levelSelected.SelectMode(true);
            if (OnLevelSelected != null)
                OnLevelSelected(_levelSelected.GetLevelScriptable());
        }

        public override void Disable()
        {
            base.Disable();
            OnLevelButtonPressed -= HandleLevelSelected;
        }

        private void InstantiateLevelsButtons(List<LevelScriptable> levels)
        {
            LevelScriptable tutorialLevel = levels.Where(l => l.TutorialLevel).FirstOrDefault();
            levels.Remove(tutorialLevel);
            levels = levels.OrderBy(l => l.LevelNumber).ToList();

            levelsButtons = GetComponentsInChildren<UI_SelectLevelButton>().ToList();
            levelsButtons[0].Init(tutorialLevel, this);

            for (int i = 0; i < levels.Count; i++)
            {
                levelsButtons[i + 1].Init(levels[i], this);
            }
        }
    }
}
