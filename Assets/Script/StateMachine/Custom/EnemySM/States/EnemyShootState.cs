using UnityEngine;
using System.Collections;
using StateMachine.EnemySM;

public class EnemyShootState : EnemySMStateBase
{
    private GameObject player;

    public override void Enter()
    {
        //Giusto per notare il cambio di stato nella build (da togliere)
        context.enemy.gameObject.GetComponentInChildren<MeshRenderer>().material.color = Color.green;
        Debug.Log("Enter Alert State");
        player = GameObject.FindGameObjectWithTag("Player");

        // Shoot

        if (player.transform.position.y == context.enemy.gameObject.transform.position.y)
        {
            context.enemy.Alert();
        }
        else
        {
            context.EndAlertCallback();
        }
    }



}
