using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;

public class UI_SelectLevelButton : MonoBehaviour
{
    [SerializeField]
    private LevelScriptable level;

    public void LevelSelected()
    {
        if (UIMenu_LevelSelection.OnLevelSelected != null)
            UIMenu_LevelSelection.OnLevelSelected(level);
    }
}
