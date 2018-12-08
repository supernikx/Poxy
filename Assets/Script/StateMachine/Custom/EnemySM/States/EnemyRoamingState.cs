using UnityEngine;
using System.Collections;

namespace StateMachine.EnemySM {

    public class EnemyRoamingState : EnemySMStateBase
    {

        private Transform leftLimit;
        private Transform rightLimit;

        /// <summary>
        /// Function that activate on state enter
        /// </summary>
        public override void Enter()
        {
            // If exists a reference to the enemy object
            if (context.enemy != null)
            {
                Debug.Log("Enter Roaming State");
                leftLimit = context.enemy.LeftLimit;
                rightLimit = context.enemy.RightLimit;
            }
        }

        /// <summary>
        /// Behaviour during Update
        /// </summary>
        public override void Tick()
        {
            Debug.Log("Tick");
        }

        /// <summary>
        /// Function that activate on state exit
        /// </summary>
        public override void Exit()
        {
            if (context.enemy != null)
            {
                Debug.Log("Leaving Roaming State");
            }
        }
    }

}

