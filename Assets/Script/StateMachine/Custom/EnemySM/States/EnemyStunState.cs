using UnityEngine;
using System.Collections;

namespace StateMachine.EnemySM {

    public class EnemyStunState : EnemySMStateBase
    {
        /// <summary>
        /// Duration of the Stun State
        /// </summary>
        private int stunDuration;
        /// <summary>
        /// true if the stun state is ready
        /// </summary>
        private bool start = false;
        /// <summary>
        /// Count the time passed since entering the state
        /// </summary>
        private float timer;

        /// <summary>
        /// Function that activate on state enter
        /// </summary>
        public override void Enter()
        {
            // If exists a reference to the enemy object
            if (context.enemy != null)
            {
                Debug.Log("Enter Stun State");
                stunDuration = context.enemy.StunDuration;
                timer = 0;
                start = true;
            }
        }

        /// <summary>
        /// Count time passed and then change state
        /// </summary>
        public override void Tick()
        {
            Debug.Log("porcoddio");
            if (start)
            {
                timer += Time.deltaTime;
                if (timer >= stunDuration)
                {
                    context.enemy.EnemySM.ChangeState("GoToRoaming");
                }
            }
        }

        /// <summary>
        /// Function that activate on state exit
        /// </summary>
        public override void Exit()
        {
            if (context.enemy != null)
            {
                Debug.Log("Leaving Stun State");
                start = false;
            }
        }

    }

}

