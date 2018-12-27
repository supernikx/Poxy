using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine.EnemySM
{
    public class EnemyAfterParasiteState : EnemySMStateBase
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
            //Giusto per notare il cambio di stato nella build (da togliere)
            context.enemy.gameObject.GetComponentInChildren<MeshRenderer>().material.color = Color.yellow;
            Debug.Log("Enter Stun State");

            stunDuration = context.enemy.GetStunDuration();
            timer = 0;
            start = true;
        }

        /// <summary>
        /// Count time passed and then change state
        /// </summary>
        public override void Tick()
        {
            if (start)
            {
                timer += Time.deltaTime;
                if (timer >= stunDuration)
                {
                    context.EndParasiteCallback();
                }
            }
        }

        /// <summary>
        /// Function that activate on state exit
        /// </summary>
        public override void Exit()
        {
            Debug.Log("Leaving Stun State");
            start = false;
        }
    }
}
