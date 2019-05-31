using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelsManager : MonoBehaviour
{
    LevelScriptable selectedLevel;
    bool speedRunMode;

    public void SetSelectedLevel(LevelScriptable _selectedLevel)
    {
        selectedLevel = _selectedLevel;
    }

    public void SetMode(bool _speedrun)
    {
        speedRunMode = _speedrun;
    }

    public LevelScriptable GetSelectedLevel()
    {
        return selectedLevel;
    }

    public bool GetMode()
    {
        return speedRunMode;
    }
}
