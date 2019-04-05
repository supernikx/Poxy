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
            context.enemy.GetVFXController().EnemyStunVFX(true);
            context.enemy.GetAnimationController().Stun(true);
            context.enemy.GetEnemyCommandsSpriteController().ToggleParasiteButton(true);
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
                    start = false;
                }
            }
        }

        /// <summary>
        /// Function that activate on state exit
        /// </summary>
        public override void Exit()
        {
            context.enemy.ResetStunHit();
            context.enemy.SetCanStun(true);
            context.enemy.GetVFXController().EnemyStunVFX(false);
            context.enemy.GetAnimationController().Stun(false);
            context.enemy.GetEnemyCommandsSpriteController().ToggleParasiteButton(false);
        }
    }
}

