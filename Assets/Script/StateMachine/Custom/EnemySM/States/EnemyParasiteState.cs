using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine.EnemySM
{
    public class EnemyParasiteState : EnemySMStateBase
    {
        public override void Enter()
        {
            Debug.Log("Enemy");

            context.enemy.gameObject.transform.parent = context.player.transform;
            context.enemy.gameObject.transform.localPosition = Vector3.zero;

            context.enemy.GetToleranceCtrl().Setup();

            //Giusto per notare il cambio di stato nella build (da togliere)
            context.enemy.gameObject.GetComponentInChildren<MeshRenderer>().material.color = Color.blue;      
        }

        public override void Tick()
        {
            if (context.enemy.GetToleranceCtrl().CheckTolerance())
            {
                context.player.Normal();
            }
            
        }

        public override void Exit()
        {
            context.enemy.GetToleranceCtrl().SetCanStart(false);
            context.enemy.gameObject.transform.parent = null;
            //Fix provvisorio per essere sulla linea della gun
            context.enemy.gameObject.transform.position = new Vector3(context.enemy.gameObject.transform.position.x, context.enemy.gameObject.transform.position.y, 1.7f);
        }
    }
}
