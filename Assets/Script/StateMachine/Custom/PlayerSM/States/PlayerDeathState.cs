using UnityEngine;
using System.Collections;
using StateMachine.PlayerSM;
using System;

public class PlayerDeathState : PlayerSMStateBase
{
    [SerializeField]
    float respawnTime;

    public override void Enter()
    {
        PlayerVFXController.OnDeathVFXEnd += HandleDeathVFXEnd;
        context.player.GetVFXController().PlayDeathVFX();

        timerForRespawn = false;
        respawnTimer = 0f;
        context.player.StopImmunityCoroutine();
        PlayerInputManager.Rumble(1f, 1f, 0.5f);
        context.player.ChangeGraphics(context.player.GetPlayerGraphic());

        context.player.GetAnimatorController().SetAnimatorController(null);

        PlayerShotController shotCtrl = context.player.GetShotController();
        shotCtrl.ChangeShotType(shotCtrl.GetPlayerDefaultShotSetting());

        context.player.GetActualGraphic().GetModel().transform.localScale = new Vector3(1, 1, 1);
        context.player.GetActualGraphic().Disable();
        context.player.GetCollisionController().OnStickyEnd();
        context.player.GetCollisionController().GetPlayerCollider().enabled = false;
        context.player.GetCollisionController().GetCollisionInfo().ResetAll();
        context.player.GetMovementController().SetCanMove(false);
        context.player.GetShotController().SetCanShoot(false);
        context.player.GetShotController().SetCanAim(false);
    }

    private void HandleDeathVFXEnd()
    {
        context.UIManager.GetGameplayManager().ToggleMenu(MenuType.Loading);
        PlayerVFXController.OnDeathVFXEnd -= HandleDeathVFXEnd;

        if (context.player.OnPlayerDeath != null)
            context.player.OnPlayerDeath();

        context.player.GetHealthController().Setup();
        context.player.transform.position = context.checkpointManager.GetActiveCheckpoint().GetPosition();
        timerForRespawn = true;
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
                context.player.GoToNormalState();
                respawnTimer = 0f;
                timerForRespawn = false;
            }
        }
    }

    public override void Exit()
    {
        context.player.GetActualGraphic().Enable();
        context.player.GetCollisionController().GetPlayerCollider().enabled = true;

        context.UIManager.GetGameplayManager().ToggleMenu(MenuType.Game);
        context.player.GetMovementController().SetCanMove(true);
        context.player.GetShotController().SetCanShoot(true);
        context.player.GetShotController().SetCanAim(true);
    }

    private void OnDestroy()
    {
        PlayerVFXController.OnDeathVFXEnd -= HandleDeathVFXEnd;
    }
}
