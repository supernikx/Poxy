using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine.EnemySM
{
    public class EnemyParasiteState : EnemySMStateBase
    {
        public override void Enter()
        {
            context.enemy.gameObject.transform.parent = context.player.transform;            
        }

        public override void Exit()
        {
            context.enemy.gameObject.transform.parent = null;
            //Fix provvisorio per essere sulla linea della gun
            context.enemy.gameObject.transform.position = new Vector3(context.enemy.gameObject.transform.position.x, context.enemy.gameObject.transform.position.y, 1.7f);
        }
    }
}
