using UnityEngine;
using System.Collections;
using StateMachine.PlayerSM;

public class PlayerDeathState : PlayerSMStateBase
{

    public override void Enter()
    {
        context.player.GetActualGraphic().SetActive(false);
        context.player.GetMovementController().SetCanMove(false);
        context.player.GetShotController().SetCanShoot(false);
        context.player.GetLivesController().LoseLives();
    }

    public override void Exit()
    {
        context.player.GetHealthController().Setup();
        context.player.GetActualGraphic().SetActive(true);
        context.player.GetMovementController().SetCanMove(true);
        context.player.GetShotController().SetCanShoot(true);
        context.player.transform.position = context.checkpointManager.GetActiveCheckpoint().GetPosition();
    }

}
