using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine.EnemySM
{
    public class EnemyParasiteState : EnemySMStateBase
    {
        EnemyToleranceController tolleranceCtrl;
        UI_GameplayManager uiManager;

        public override void Enter()
        {
            uiManager = context.UIManager;
            tolleranceCtrl = context.enemy.GetToleranceCtrl();
            tolleranceCtrl.Setup();
            uiManager.GetGamePanel().SetMaxToleranceValue(tolleranceCtrl.GetMaxTolerance());
            uiManager.GetGamePanel().EnableToleranceBar(true);
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

            uiManager.GetGamePanel().EnableToleranceBar(false);

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
