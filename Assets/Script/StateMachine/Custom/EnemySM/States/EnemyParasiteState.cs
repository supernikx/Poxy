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
            context.enemy.gameObject.transform.parent = context.player.transform;
            context.enemy.gameObject.transform.localPosition = Vector3.zero;

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

                if (tolleranceCtrl.CheckTollerance())
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

            context.enemy.gameObject.transform.parent = null;
            //Fix provvisorio per essere sulla linea della gun
            context.enemy.gameObject.transform.position = new Vector3(context.enemy.gameObject.transform.position.x, context.enemy.gameObject.transform.position.y, 1.7f);
        }

        private void PlayerMaxHealth()
        {
            Debug.Log("Player max health");
            tolleranceCtrl.SetActive(true);
        }
    }
}
