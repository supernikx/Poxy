using UnityEngine;
using System.Collections;
using StateMachine.PlayerSM;

public class PlayerDeathState : PlayerSMStateBase
{

    public override void Enter()
    {
        Debug.Log("Player Death State Enter");
        context.player.gameObject.SetActive(false);
    }

}
