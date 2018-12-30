using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine.EnemySM;

public class EnemyAlertState : EnemySMStateBase
{

    public override void Enter()
    {
        //Giusto per notare il cambio di stato nella build (da togliere)
        context.enemy.gameObject.GetComponentInChildren<MeshRenderer>().material.color = Color.green;
        Debug.Log("Enter Alert State");
    }

    public override void Tick()
    {
        if (!context.enemy.AlertActions())
        {
            context.EndAlertCallback();
        }
        
    }

    public override void Exit()
    {
        base.Exit();
    }

}
