using UnityEngine;
using System.Collections;

namespace StateMachine.EnemySM
{

    public class EnemyDeadState : EnemySMStateBase
    {
        /// <summary>
        /// Death State duration
        /// </summary>
        private int deathDuration;

        /// <summary>
        /// Function that activate on state enter
        /// </summary>
        public override void Enter()
        {
            // If exists a reference to the enemy object
            if (context.enemy != null)
            {
                Debug.Log("Enter Dead State");
                deathDuration = context.enemy.DeathDuration;
            }
        }

        /// <summary>
        /// Function that activate on state exit
        /// </summary>
        public override void Exit()
        {
            if (context.enemy != null)
            {
                Debug.Log("Leaving Dead State");
            }
        }

    }

}

