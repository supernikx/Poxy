using UnityEngine;
using System.Collections;
using StateMachine.PlayerSM;
using System;

public class PlayerDeathState : PlayerSMStateBase
{

    public override void Enter()
    {
        PlayerVFXController.OnDeathVFXEnd += HandleDeathVFXEnd;
        context.player.GetActualGraphic().SetActive(false);
        context.player.GetCollisionController().OnStickyEnd();
        context.player.GetCollisionController().GetPlayerCollider().enabled = false;
        context.player.GetCollisionController().GetCollisionInfo().ResetAll();
        context.player.GetMovementController().SetCanMove(false);
        context.player.GetShotController().SetCanShoot(false);
        context.player.GetLivesController().LoseLives();
    }

    private void HandleDeathVFXEnd()
    {
        context.player.GoToNormalState();
    }

    public override void Exit()
    {
        context.player.GetCollisionController().GetPlayerCollider().enabled = true;
        context.player.GetHealthController().Setup();
        context.player.GetActualGraphic().SetActive(true);
        context.player.GetMovementController().SetCanMove(true);
        context.player.GetShotController().SetCanShoot(true);
        context.player.transform.position = context.checkpointManager.GetActiveCheckpoint().GetPosition();
    }

}
