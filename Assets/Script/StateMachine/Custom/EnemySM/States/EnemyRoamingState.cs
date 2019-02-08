using UnityEngine;
using System.Collections;

namespace StateMachine.EnemySM
{

    public class EnemyRoamingState : EnemySMStateBase
    {
        private EnemyViewController viewCtrl;

        /// <summary>
        /// Function that activate on state enter
        /// </summary>
        public override void Enter()
        {
            viewCtrl = context.enemy.GetViewCtrl();

            context.enemy.MoveRoaming(true);
        }

        /// <summary>
        /// Behaviour during Update
        /// </summary>
        public override void Tick()
        {
            Transform playerTransform = viewCtrl.FindPlayer();
            if (playerTransform != null)
            {
                if (viewCtrl.CanSeePlayer(playerTransform.position))
                {
                    context.enemy.Alert();
                }
            }
        }

        /// <summary>
        /// Function that activate on state exit
        /// </summary>
        public override void Exit()
        {
            context.enemy.MoveRoaming(false);
        }
    }

}

