using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine.EnemySM;

public class EnemyAlertState : EnemySMStateBase
{
    private EnemyViewController viewCtrl;

    public override void Enter()
    {
        viewCtrl = context.enemy.GetViewCtrl();
        context.enemy.AlertActions(true);
    }

    public override void Tick()
    {      
        Transform playerTransform = viewCtrl.FindPlayer();
        if (playerTransform == null)
        {
            context.EndAlertCallback();
        }        
    }

    public override void Exit()
    {
        context.enemy.AlertActions(false);
    }

}
