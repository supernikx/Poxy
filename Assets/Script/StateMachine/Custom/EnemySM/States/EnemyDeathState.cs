using UnityEngine;
using System.Collections;

namespace StateMachine.EnemySM
{

    public class EnemyDeathState : EnemySMStateBase
    {
        /// <summary>
        /// Death State duration
        /// </summary>
        private float deathDuration;
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
            deathDuration = context.enemy.GetRespawnDuration();
            timer = 0;
            context.enemy.GetCollider().enabled = false;
            context.enemy.GetCollisionCtrl().OnStickyEnd();
            context.enemy.GetCollisionCtrl().GetCollisionInfo().ResetAll();
            context.enemy.ResetLife();
            context.enemy.ResetStunHit();
            context.enemy.SetCanStun(false);
            start = true;

            if (deathDuration != 0 && context.enemy.GetGraphics().GetModel().activeSelf)
                context.enemy.GetAnimationController().DeathAnimation(HandleDeathAnimationEnd);
        }

        private void HandleDeathAnimationEnd()
        {
            context.enemy.GetAnimationController().ResetAnimator();
            context.enemy.GetVFXController().EnemyDeathVFX();
            context.enemy.GetGraphics().Disable();
        }

        /// <summary>
        /// Count time passed and then change state
        /// </summary>
        public override void Tick()
        {
            if (start && deathDuration >= 0)
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
            if (context.enemy.gameObject.transform.parent != null && context.enemy.gameObject.transform.parent != context.enemy.GetEnemyDefaultParent())
                context.enemy.gameObject.transform.parent = context.enemy.GetEnemyDefaultParent();
            context.enemy.ResetPosition();
            start = false;
            context.enemy.SetCanStun(true);
            context.enemy.GetGraphics().Enable();
            context.enemy.GetCollider().enabled = true;
        }
    }
}

