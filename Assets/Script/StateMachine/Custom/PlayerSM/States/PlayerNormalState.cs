using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine.PlayerSM
{
    public class PlayerNormalState : PlayerSMStateBase
    {
        public override void Enter()
        {
            context.player.GetShootController().SetCanShoot(false);
            context.player.GetMovementController().SetCanMove(true);            
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void Tick()
        {
            if (Input.GetButtonDown("Parasite"))
            {
                IEnemy e = context.player.GetParasiteController().CheckParasite();
                if (e != null)
                {
                    context.player.Parasite(e);
                }
            }
        }
    }
}
