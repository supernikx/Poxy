using UnityEngine;
using System.Collections;

namespace StateMachine.EnemySM {

    public class EnemyIdleState : EnemySMStateBase {

        /// <summary>
        /// Function that activate on state enter
        /// </summary>
        public override void Enter()
        {
            // If exists a reference to the enemy object
            if (context.enemy != null)
            {
                Debug.Log("Enter Idle State");
            }
        }

        /// <summary>
        /// Function that activate on state exit
        /// </summary>
        public override void Exit()
        {
            if (context.enemy != null)
            {
                Debug.Log("Leaving Idle State");
            }
        }

    }

}

