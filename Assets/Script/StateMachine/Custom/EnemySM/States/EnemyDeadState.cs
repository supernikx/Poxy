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
        /// Bool that set when the timer starts
        /// </summary>
        private bool start = false;
        /// <summary>
        /// Internal timer that counts time passed
        /// </summary>
        private float timer = 0;

        /// <summary>
        /// Function that activate on state enter
        /// </summary>
        public override void Enter()
        {
            // If exists a reference to the enemy object
            deathDuration = context.enemy.GetDeathDuration();
            timer = 0;
            context.enemy.GetGraphics().SetActive(false);
            context.enemy.GetCollider().enabled = false;

            context.enemy.ResetPosition();
            context.enemy.ResetData();

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
                if (timer >= deathDuration)
                {
                    context.EndDeathCallback();
                }
            }
        }

        /// <summary>
        /// Function that activate on state exit
        /// </summary>
        public override void Exit()
        {
            start = false;
            context.enemy.GetGraphics().SetActive(true);
            context.enemy.GetCollider().enabled = true;
        }
    }

}

