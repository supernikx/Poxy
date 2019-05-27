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
    }

    private void HandleLevelSelected(LevelScriptable _selectedLevel)
    {
        context.gameManager.GetLevelsManager().SetSelectedLevel(_selectedLevel);
        GameManager.StartGame();
    }

    public override void Exit()
    {
        UIMenu_LevelSelection.OnLevelSelected -= HandleLevelSelected;
        context.gameManager.GetUIManager().ToggleMenu(MenuType.None);
    }
}
