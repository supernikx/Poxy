using UnityEngine;
using System.Collections;
using StateMachine.PlayerSM;

public class PlayerDeathState : PlayerSMStateBase
{

    public override void Enter()
    {
        context.player.gameObject.SetActive(false);
    }

}
