using UnityEngine;
using System.Collections;
using StateMachine.PlayerSM;

public class PlayerDeathState : PlayerSMStateBase
{

    public override void Enter()
    {
        context.player.GetActualGraphic().SetActive(false);
        context.player.GetMovementController().SetCanMove(false);
        Vector3 _respawnPosition = context.player.GetLivesController().LoseLife();
        context.player.transform.position = _respawnPosition;
        context.player.GoToNormalState();
    }

    public override void Exit()
    {
        context.player.GetHealthController().Setup();
        context.player.GetActualGraphic().SetActive(true);
    }

}
