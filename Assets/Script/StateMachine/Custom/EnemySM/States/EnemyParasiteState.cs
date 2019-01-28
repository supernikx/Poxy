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

            context.player.OnPlayerMaxHealth += PlayerMaxHealth;

            //Giusto per notare il cambio di stato nella build (da togliere)
            context.enemy.gameObject.GetComponentInChildren<MeshRenderer>().material.color = Color.blue;
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

            context.enemy.gameObject.transform.parent = context.enemy.GetEnemyDefaultParent();
            context.enemy.gameObject.layer = context.enemy.GetEnemyDefaultLayer();
        }

        private void PlayerMaxHealth()
        {
            tolleranceCtrl.SetActive(true);
        }
    }
}
