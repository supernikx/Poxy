using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine.PlayerSM
{
    public class PlayerParasiteState : PlayerSMStateBase
    {
        public override void Enter()
        {
            context.player.transform.position = new Vector3(context.parasiteEnemy.gameObject.transform.position.x, context.parasiteEnemy.gameObject.transform.position.y, context.player.transform.position.z);
            context.parasiteEnemy.gameObject.transform.parent = context.player.transform;
            context.parasiteEnemy.gameObject.transform.localPosition = Vector3.zero;
            context.player.EnableGraphics(false);
            context.player.GetCollisionController().CalculateParasiteCollision(context.parasiteEnemy);

            context.player.GetShootController().SetCanShoot(true);
        }

        public override void Tick()
        {
            if (Input.GetButtonDown("Parasite"))
            {
                context.player.Normal();
            }
        }

        public override void Exit()
        {
            //Fix provvisorio altrimenti non detecta collision
            context.player.transform.position = new Vector3(context.player.transform.position.x, context.player.transform.position.y + 0.5f, context.player.transform.position.z);
            context.player.EnableGraphics(true);
            context.player.GetCollisionController().CalculateNormalCollision();
        }
    }
}
