using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine.PlayerSM
{
    public class PlayerNormalState : PlayerSMStateBase
    {
        public override void Enter()
        {
            if (context.player != null)
            {
                context.player.GetShootController().SetCanShoot(true);
                context.player.GetMovementController().SetCanMove(true);
            }
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}
