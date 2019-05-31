using System;
using StateMachine.GameSM;
using UI;
using UnityEngine;

public class GameLevelSelectionState : GameSMStateBase
{
    public override void Enter()
    {
        context.gameManager.GetUIManager().ToggleMenu(MenuType.LevelSelection);
        UIMenu_LevelSelection.OnLevelSelected += HandleLevelSelected;
        UIMenu_LevelSelection.OnModeSelected += HandleOnModeSelected;
    }

    private void HandleLevelSelected(LevelScriptable _selectedLevel)
    {
        context.gameManager.GetLevelsManager().SetSelectedLevel(_selectedLevel);
        if (_selectedLevel.TutorialLevel)
            GameManager.StartGame();
    }
    private void HandleOnModeSelected(bool _speedRun)
    {
        context.gameManager.GetLevelsManager().SetMode(_speedRun);
        GameManager.StartGame();
    }

    public override void Exit()
    {
        UIMenu_LevelSelection.OnLevelSelected -= HandleLevelSelected;
        UIMenu_LevelSelection.OnModeSelected -= HandleOnModeSelected;
        context.gameManager.GetUIManager().ToggleMenu(MenuType.None);
    }
}
