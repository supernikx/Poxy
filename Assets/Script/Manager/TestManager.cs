using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestManager : MonoBehaviour
{
    public static TestManager instance;
    public bool speedrunMode = false;

    UI_ManagerBase ui;
    SoundManager soundMng;
    LevelManager lvl;

    private void Start()
    {
        ui = FindObjectOfType<UI_ManagerBase>();
        GameManager gm = FindObjectOfType<GameManager>();
        lvl = FindObjectOfType<LevelManager>();
        soundMng = GetComponent<SoundManager>();
        if (gm == null && lvl != null)
        {
            instance = this;
            ui.Setup(gm);
            soundMng.Init();
            lvl.Init(ui, speedrunMode);
            if (speedrunMode)
            {
                PlayerInputManager.SetCanReadInput(false);
                lvl.GetPlayer().GetHealthController().SetCanLoseHealth(false);

                ui.GetGameplayManager().GetCountdownPanel().OnCountdownEnd += HandleOnCountdownEnd;
                ui.ToggleMenu(MenuType.Countdown);
            }
            else
            {
                PlayerInputManager.SetCanReadInput(true);
                ui.ToggleMenu(MenuType.Game);
            }
        }
        else
            DestroyImmediate(gameObject);
    }

    private void HandleOnCountdownEnd()
    {
        PlayerInputManager.SetCanReadInput(true);
        lvl.GetPlayer().GetHealthController().SetCanLoseHealth(true);
        ui.ToggleMenu(MenuType.Game);

        if (SpeedrunManager.StartTimer != null)
            SpeedrunManager.StartTimer();
    }

    public SoundManager GetSoundManager()
    {
        return soundMng;
    }

    private void OnDisable()
    {
        ui.GetGameplayManager().GetCountdownPanel().OnCountdownEnd -= HandleOnCountdownEnd;
    }
}
