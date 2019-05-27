using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelsManager : MonoBehaviour
{
    LevelScriptable selectedLevel;

    public void SetSelectedLevel(LevelScriptable _selectedLevel)
    {
        selectedLevel = _selectedLevel;
    }

    public LevelScriptable GetSelectedLevel()
    {
        return selectedLevel;
    }
}
