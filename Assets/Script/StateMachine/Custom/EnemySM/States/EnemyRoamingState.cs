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
            Debug.Log("Enter Roaming State");
        }

        /// <summary>
        /// Behaviour during Update
        /// </summary>
        public override void Tick()
        {
            context.enemy.Move();
        }

        /// <summary>
        /// Function that activate on state exit
        /// </summary>
        public override void Exit()
        {
            Debug.Log("Leaving Roaming State");
        }
    }

}

