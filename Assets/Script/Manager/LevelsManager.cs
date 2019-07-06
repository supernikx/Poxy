using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelsManager : MonoBehaviour
{
    [SerializeField]
    private List<LevelScriptable> levelsOrder = new List<LevelScriptable>();

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

    public LevelScriptable GetNextLevel(LevelScriptable _level)
    {
        LevelScriptable nextLevel = null;
        for (int i = 0; i < levelsOrder.Count; i++)
        {
            if (_level == levelsOrder[i])
            {
                if (i < levelsOrder.Count - 1)
                    nextLevel = levelsOrder[i + 1];
            }
        }

        return nextLevel;
    }
}
