using UnityEngine;
using System.Collections;

namespace StateMachine.EnemySM
{

    public class EnemyRoamingState : EnemySMStateBase
    {
        /// <summary>
        /// Function that activate on state enter
        /// </summary>
        public override void Enter()
        {
            //Giusto per notare il cambio di stato nella build (da togliere)
            context.enemy.gameObject.GetComponentInChildren<MeshRenderer>().material.color = Color.red;
            Debug.Log("Enter Roaming State");
            context.enemy.Move();
        }

        /// <summary>
        /// Behaviour during Update
        /// </summary>
        public override void Tick()
        {
        }

        /// <summary>
        /// Function that activate on state exit
        /// </summary>
        public override void Exit()
        {
            Debug.Log("Leaving Roaming State");
            context.enemy.Stop();
        }
    }

}

