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
            lvl.Init(ui, speedrunMode);
            if (speedrunMode)
                ui.ToggleMenu(MenuType.Countdown);
            else
                ui.ToggleMenu(MenuType.Game);
        }
        else
            DestroyImmediate(gameObject);
    }
}
