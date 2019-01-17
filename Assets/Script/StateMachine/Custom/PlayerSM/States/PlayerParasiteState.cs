using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine.PlayerSM
{
    public class PlayerParasiteState : PlayerSMStateBase
    {
        private bool healthMax;

        public override void Enter()
        {
            context.player.OnEnemyCollision += OnEnemyCollision;
            context.parasiteEnemy.GetToleranceCtrl().OnMaxTolleranceBar += OnMaxTolleranceBar;

            parasitePressed = false;
            healthMax = false;
            ltWasDown = true; //Imposta il tasto del controller come premuto

            context.parasiteEnemy.gameObject.transform.parent = context.player.transform;
            context.parasiteEnemy.gameObject.transform.localPosition = Vector3.zero;
            context.parasiteEnemy.gameObject.transform.localRotation = Quaternion.identity;
            context.parasiteEnemy.gameObject.layer = context.player.gameObject.layer;

            context.player.GetCollisionController().CalculateParasiteCollision(context.parasiteEnemy);
            context.player.GetMovementController().SetCanMove(true);
            context.player.GetShotController().SetCanShootDamage(true);
        }

        bool parasitePressed;
        public override void Tick()
        {
            if (Input.GetButtonDown("Parasite") || CheckJoystickLTAxis() && !parasitePressed)
            {
                parasitePressed = true;
                context.player.StartNormalCoroutine();
            }

            if (Input.GetButtonDown("RightMouse") || CheckJoystickRTAxis())
            {
                //Cambio tipo di sparo
                context.player.GetShotController().ChangeShotType();
            }

            if (context.player.GetHealthController().GainHealthOvertime() && !healthMax)
            {
                healthMax = true;
                if (context.player.OnPlayerMaxHealth != null)
                    context.player.OnPlayerMaxHealth();
            }
        }

        public override void Exit()
        {
            context.player.OnEnemyCollision -= OnEnemyCollision;
            context.parasiteEnemy.GetToleranceCtrl().OnMaxTolleranceBar -= OnMaxTolleranceBar;
            context.player.GetParasiteController().SetParasiteEnemy(null);

            //Fix provvisorio altrimenti non detecta collision
            context.player.transform.position = new Vector3(context.player.transform.position.x, context.player.transform.position.y + 0.5f, context.player.transform.position.z);
            context.player.GetCollisionController().CalculateNormalCollision();

            context.player.GetShotController().SetCanShootDamage(false);
            parasitePressed = false;
            healthMax = false;
        }

        private void OnMaxTolleranceBar()
        {
            context.player.StartNormalCoroutine();
        }

        #region EnemyCollision
        private void OnEnemyCollision(IEnemy _enemy)
        {
            context.player.OnEnemyCollision -= OnEnemyCollision;
            context.parasiteEnemy.GetToleranceCtrl().AddTollerance(_enemy.GetDamage());
            context.player.OnPlayerImmunityEnd += PlayerImmunityEnd;
            context.player.StartImmunityCoroutine(context.player.GetCollisionController().GetImmunityDuration());
        }

        private void PlayerImmunityEnd()
        {
            context.player.OnEnemyCollision += OnEnemyCollision;
            context.player.OnPlayerImmunityEnd -= PlayerImmunityEnd;
        }
        #endregion
    }
}
