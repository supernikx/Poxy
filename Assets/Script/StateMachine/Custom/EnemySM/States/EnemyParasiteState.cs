using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine.EnemySM
{
    public class EnemyParasiteState : EnemySMStateBase
    {
        EnemyToleranceController tolleranceCtrl;

        public override void Enter()
        {
            tolleranceCtrl = context.enemy.GetToleranceCtrl();
            tolleranceCtrl.Setup();
            context.enemy.SetCanStun(false);
            context.player.OnPlayerMaxHealth += PlayerMaxHealth;
        }

        public override void Tick()
        {
            if (tolleranceCtrl.IsActive())
            {
                tolleranceCtrl.AddTolleranceOvertime();

                if (tolleranceCtrl.CheckTolerance())
                {
                    if (tolleranceCtrl.OnMaxTolleranceBar != null)
                        tolleranceCtrl.OnMaxTolleranceBar();
                }
            }
        }

        public override void Exit()
        {
            context.player.OnPlayerMaxHealth -= PlayerMaxHealth;
            context.player = null;

            tolleranceCtrl.SetActive(false);
            tolleranceCtrl = null;

            context.enemy.SetCanStun(true);
            context.enemy.gameObject.transform.parent = context.enemy.GetEnemyDefaultParent();
            context.enemy.gameObject.layer = context.enemy.GetEnemyDefaultLayer();
        }

        private void PlayerMaxHealth()
        {
            tolleranceCtrl.SetActive(true);
        }
    }
}
