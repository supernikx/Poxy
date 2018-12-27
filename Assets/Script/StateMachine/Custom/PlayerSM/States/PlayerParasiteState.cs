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
            context.player.Normal();
        }

        #region EnemyCollision
        bool _playerImmunity;
        float immunityTime;
        private void OnEnemyCollision(IEnemy _enemy)
        {
            context.player.OnEnemyCollision -= OnEnemyCollision;
            context.parasiteEnemy.GetToleranceCtrl().AddTollerance(_enemy.GetDamage());
            context.player.GetCollisionController().CheckEnemyCollision(false);
            context.player.gameObject.layer = LayerMask.NameToLayer("PlayerImmunity");

            immunityTime = context.player.GetCollisionController().GetImmunityDuration();
            _playerImmunity = true;
        }

        private void PlayerImmunityEnd()
        {
            _playerImmunity = false;
            context.player.OnEnemyCollision += OnEnemyCollision;
            context.player.GetCollisionController().CheckEnemyCollision(true);
            context.player.gameObject.layer = LayerMask.NameToLayer("Player");
        }
        #endregion
    }
}
