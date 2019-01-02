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

            healthMax = false;
            ltWasDown = true; //Imposta il tasto del controller come premuto

            context.parasiteEnemy.gameObject.transform.parent = context.player.transform;
            context.parasiteEnemy.gameObject.transform.localPosition = Vector3.zero;

            context.player.EnableGraphics(false);
            context.player.GetCollisionController().CalculateParasiteCollision(context.parasiteEnemy);

            context.player.GetShotController().SetCanShoot(true);
        }

        public override void Tick()
        {
            if (Input.GetButtonDown("Parasite") || CheckJoystickLTAxis())
            {
                context.player.StartNormalCoroutine();
            }

            if (Input.GetButtonDown("RightMouse") || CheckJoystickRTAxis())
            {
                //Cambio tipo di sparo
                context.player.GetShotController().ChangeShotType();
                Debug.Log("Destro");
            }

            if (context.player.GetHealthController().GainHealthOvertime() && !healthMax)
            {
                healthMax = true;
                if (context.player.OnPlayerMaxHealth != null)
                    context.player.OnPlayerMaxHealth();
            }

            if (_playerImmunity)
            {
                if (immunityTime <= 0)
                    PlayerImmunityEnd();

                immunityTime -= Time.deltaTime;
            }
        }

        public override void Exit()
        {
            context.player.OnEnemyCollision -= OnEnemyCollision;
            context.parasiteEnemy.GetToleranceCtrl().OnMaxTolleranceBar -= OnMaxTolleranceBar;
            context.player.GetParasiteController().SetParasiteEnemy(null);

            //Fix provvisorio altrimenti non detecta collision
            context.player.transform.position = new Vector3(context.player.transform.position.x, context.player.transform.position.y + 0.5f, context.player.transform.position.z);
            context.player.EnableGraphics(true);
            context.player.GetCollisionController().CalculateNormalCollision();
            healthMax = false;
        }

        private void OnMaxTolleranceBar()
        {
            context.player.StartNormalCoroutine();
        }

        #region EnemyCollision
        bool _playerImmunity;
        float immunityTime;
        private void OnEnemyCollision(IEnemy _enemy)
        {
            context.player.OnEnemyCollision -= OnEnemyCollision;
            context.parasiteEnemy.GetToleranceCtrl().AddTollerance(_enemy.GetDamage());
            context.player.GetCollisionController().CheckDamageCollision(false);
            context.player.gameObject.layer = LayerMask.NameToLayer("PlayerImmunity");

            immunityTime = context.player.GetCollisionController().GetImmunityDuration();
            _playerImmunity = true;
        }

        private void PlayerImmunityEnd()
        {
            _playerImmunity = false;
            context.player.OnEnemyCollision += OnEnemyCollision;
            context.player.GetCollisionController().CheckDamageCollision(true);
            context.player.gameObject.layer = LayerMask.NameToLayer("Player");
        }
        #endregion
    }
}
