using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") || other.gameObject.layer == LayerMask.NameToLayer("PlayerImmunity"))
        {
            Time.timeScale = 0f;

            if (GameManager.Exist())
            {
                if (GameManager.instance.GetLevelsManager().GetMode())
                {
                    OptionsManager optionsMng = GameManager.instance.GetOptionsManager();
                    SpeedrunManager speedRunMng = LevelManager.instance.GetSpeedrunManager();
                    dreamloLeaderBoard leaderBoardMng = GameManager.instance.GetLeaderboard();
                    leaderBoardMng.AddScore(optionsMng.GetUserName(), speedRunMng.GetTimer());
                }
            }
            PlayerInputManager.SetCanReadInput(false);
            LevelManager.instance.GetUIGameplayManager().ToggleMenu(MenuType.EndGame);
        }
    }
}
