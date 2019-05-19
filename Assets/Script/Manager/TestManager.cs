using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestManager : MonoBehaviour
{
    public bool speedrunMode = false;

    private void Awake()
    {
        UI_ManagerBase ui = FindObjectOfType<UI_ManagerBase>();
        GameManager gm = FindObjectOfType<GameManager>();
        LevelManager lvl = FindObjectOfType<LevelManager>();
        if (gm == null && lvl != null)
        {
            ui.Setup(gm);
            ui.ToggleMenu(MenuType.Game);
            lvl.Init(ui, speedrunMode);
        }
        else
            DestroyImmediate(gameObject);
    }
}
