using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NewLevel", menuName = "Levels/NewLevel")]
public class LevelScriptable : ScriptableObject
{
    public string LevelName;
    public string SceneName;
    public int LevelNumber;
    public bool TutorialLevel;
    public bool LockedLevel;
}
