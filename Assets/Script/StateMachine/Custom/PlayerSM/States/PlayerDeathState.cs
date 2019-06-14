using UnityEngine;
using System.Collections;
using StateMachine.PlayerSM;
using System;

public class PlayerDeathState : PlayerSMStateBase
{
    [SerializeField]
    float respawnTime;

    Player player;

    public override void Enter()
    {
        player = context.player;

        PlayerVFXController.OnDeathVFXEnd += HandleDeathVFXEnd;
        player.GetVFXController().PlayDeathVFX();
        player.GetSoundController().Death();

        timerForRespawn = false;
        respawnTimer = 0f;
        player.StopImmunityCoroutine();
        PlayerInputManager.Rumble(1f, 1f, 0.5f);
        player.ChangeGraphics(context.player.GetPlayerGraphic());

        player.GetAnimatorController().SetAnimatorController(null);
        player.GetAnimatorController().ResetAnimator();

        PlayerShotController shotCtrl = context.player.GetShotController();
        shotCtrl.ChangeShotType(shotCtrl.GetPlayerDefaultShotSetting());

        player.GetActualGraphic().GetModel().transform.localScale = new Vector3(1, 1, 1);
        player.GetActualGraphic().Disable();
        player.GetCollisionController().OnStickyEnd();
        player.GetCollisionController().GetPlayerCollider().enabled = false;
        player.GetCollisionController().GetCollisionInfo().ResetAll();
        player.GetMovementController().SetCanMove(false);
        shotCtrl.SetCanShoot(false);
        shotCtrl.SetCanUseCrossair(false);
        shotCtrl.SetCanAim(false);

        if (SpeedrunManager.StopTimer != null)
            SpeedrunManager.StopTimer();
    }

    private void HandleDeathVFXEnd()
    {
        PlayerVFXController.OnDeathVFXEnd -= HandleDeathVFXEnd;

        player.GetHealthController().Setup();
        player.transform.position = context.checkpointManager.GetActiveCheckpoint().GetPosition();
        timerForRespawn = true;

        context.UIManager.GetGameplayManager().ToggleMenu(MenuType.Loading);

        if (context.player.OnPlayerDeath != null)
            context.player.OnPlayerDeath();
    }

    bool timerForRespawn;
    float respawnTimer;
    public override void Tick()
    {
        if (timerForRespawn)
        {
            respawnTimer += Time.deltaTime;
            if (respawnTimer >= respawnTime)
            {
                player.GoToNormalState();
                respawnTimer = 0f;
                timerForRespawn = false;
            }
        }
    }

    public override void Exit()
    {
        player.GetActualGraphic().Enable();
        player.GetCollisionController().GetPlayerCollider().enabled = true;

        if (LevelManager.instance.GetSpeedrunManager().GetIsActive())
            context.UIManager.GetGameplayManager().ToggleMenu(MenuType.Countdown);
        else
            context.UIManager.GetGameplayManager().ToggleMenu(MenuType.Game);
        player.GetMovementController().SetCanMove(true);
        player.GetShotController().SetCanShoot(true);
        player.GetShotController().SetCanUseCrossair(true);
        player.GetShotController().SetCanAim(true);
    }

    private void OnDestroy()
    {
        PlayerVFXController.OnDeathVFXEnd -= HandleDeathVFXEnd;
    }
}
