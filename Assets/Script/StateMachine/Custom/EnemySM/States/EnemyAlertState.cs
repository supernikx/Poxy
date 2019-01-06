using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine.EnemySM;

public class EnemyAlertState : EnemySMStateBase
{
    private Transform _playerTransform;
    private EnemyViewController viewCtrl;

    public override void Enter()
    {
        //Giusto per notare il cambio di stato nella build (da togliere)
        context.enemy.gameObject.GetComponentInChildren<MeshRenderer>().material.color = Color.green;

        viewCtrl = context.enemy.GetViewCtrl();
        _playerTransform = LevelManager.singleton.GetPlayerTransform();
    }

    public override void Tick()
    {
        context.enemy.AlertActions();

        if (!viewCtrl.CheckPlayerDistance(_playerTransform.position))
        {
            context.EndAlertCallback();
        }        
    }

    public override void Exit()
    {
        base.Exit();
    }

}
