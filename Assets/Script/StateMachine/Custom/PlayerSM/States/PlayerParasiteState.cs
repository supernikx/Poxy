﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine.PlayerSM
{
    public class PlayerParasiteState : PlayerSMStateBase
    {
        private bool firstTime;

        public override void Enter()
        {
            Debug.Log("Player");

            firstTime = true;

            context.player.transform.position = new Vector3(context.parasiteEnemy.gameObject.transform.position.x, context.parasiteEnemy.gameObject.transform.position.y, context.player.transform.position.z);

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

            if (context.player.GetHealthController().GainHealth() && firstTime)
            {
                firstTime = false;
                context.parasiteEnemy.StartTolerance();
            }
        }

        public override void Exit()
        {
            //Fix provvisorio altrimenti non detecta collision
            context.player.transform.position = new Vector3(context.player.transform.position.x, context.player.transform.position.y + 0.5f, context.player.transform.position.z);
            context.player.EnableGraphics(true);
            context.player.GetCollisionController().CalculateNormalCollision();
            firstTime = false;
        }
    }
}
