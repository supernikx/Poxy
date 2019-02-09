using UnityEngine;
using System.Collections;

namespace StateMachine.EnemySM
{
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
            context.enemy.SetCanStun(false);
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
                context.enemy.GetMovementCtrl().MovementCheck();
                timer += Time.deltaTime;
                if (timer >= stunDuration)
                {
                    context.EndStunCallback();
                }
            }
        }

        /// <summary>
        /// Function that activate on state exit
        /// </summary>
        public override void Exit()
        {
            start = false;
            context.enemy.ResetStunHit();
            context.enemy.SetCanStun(true);
        }
    }
}

