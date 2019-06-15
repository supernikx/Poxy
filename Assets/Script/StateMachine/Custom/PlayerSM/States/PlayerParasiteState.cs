﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine.PlayerSM
{
    public class PlayerParasiteState : PlayerSMStateBase
    {
        [SerializeField]
        private float parasiteTimeDelay;

        private IEnemy parasiteEnemy;

        public override void Enter()
        {
            parasiteEnemy = context.parasite as IEnemy;

            PlayerInputManager.OnParasitePressed += HandleOnPlayerParasitePressed;
            PlayerInputManager.DelayParasiteButtonPress(parasiteTimeDelay);
            parasiteEnemy.GetToleranceCtrl().OnMaxTolleranceBar += HandleOnMaxTolleranceBar;
            context.player.OnDamageableCollision += HandleOnDamageableCollision;
            parasiteEnemy.gameObject.layer = context.player.gameObject.layer;
            if (context.player.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                context.player.OnEnemyCollision += HandleOnEnemyCollision;
                gainHealth = true;
                immunity = false;
            }
            else
            {
                context.player.OnPlayerImmunityEnd += HandlePlayerImmunityEnd;
                gainHealth = false;
                immunity = true;
            }

            parasitePressed = false;

            parasiteEnemy.gameObject.transform.parent = context.player.transform;
            parasiteEnemy.gameObject.transform.localPosition = Vector3.zero;
            parasiteEnemy.gameObject.transform.localRotation = Quaternion.identity;

            context.player.GetCollisionController().CalculateParasiteCollision(parasiteEnemy);
            context.player.GetMovementController().SetCanMove(true);
            context.player.GetShotController().SetShotPoint(parasiteEnemy.GetShotPoint());
        }

        public override void Tick()
        {
            if (context.player.GetHealthController().GainHealthOverTime() && gainHealth && !parasitePressed)
            {
                gainHealth = false;
                if (context.player.OnPlayerMaxHealth != null)
                    context.player.OnPlayerMaxHealth();
            }
        }

        private void HandleOnMaxTolleranceBar()
        {
            context.player.StartNormalCoroutine();
        }

        bool parasitePressed;
        private void HandleOnPlayerParasitePressed()
        {
            if (parasitePressed)
                return;

            parasitePressed = true;
            context.player.StartNormalCoroutine();
        }

        #region Enemy/Damageable Collision
        bool immunity;
        bool gainHealth;
        private void HandleOnEnemyCollision(IEnemy _enemy)
        {
            context.player.OnEnemyCollision -= HandleOnEnemyCollision;
            parasiteEnemy.GetToleranceCtrl().AddTolerance(_enemy.GetDamage());
            context.player.OnPlayerImmunityEnd += HandlePlayerImmunityEnd;
            parasiteEnemy.gameObject.layer = LayerMask.NameToLayer("PlayerImmunity");
            context.player.StartImmunityCoroutine(context.player.GetCollisionController().GetImmunityDuration());
            immunity = true;
            gainHealth = false;
        }

        private void HandleOnDamageableCollision(IDamageable _damageable)
        {
            context.player.OnEnemyCollision -= HandleOnEnemyCollision;
            context.player.OnDamageableCollision -= HandleOnDamageableCollision;

            switch (_damageable.DamageableType)
            {
                case DamageableType.Spike:
                case DamageableType.Poop:
                case DamageableType.Acid:
                    context.player.Death();
                    break;
            }

            gainHealth = false;
            immunity = true;
        }

        private void HandlePlayerImmunityEnd()
        {
            context.player.OnEnemyCollision += HandleOnEnemyCollision;
            context.player.OnPlayerImmunityEnd -= HandlePlayerImmunityEnd;
            parasiteEnemy.gameObject.layer = LayerMask.NameToLayer("Player");

            immunity = false;
            gainHealth = true;
        }
        #endregion

        public override void Exit()
        {
            context.player.OnEnemyCollision -= HandleOnEnemyCollision;
            context.player.OnDamageableCollision -= HandleOnDamageableCollision;
            context.player.OnPlayerImmunityEnd -= HandlePlayerImmunityEnd;
            PlayerInputManager.OnParasitePressed -= HandleOnPlayerParasitePressed;

            parasiteEnemy.GetToleranceCtrl().OnMaxTolleranceBar -= HandleOnMaxTolleranceBar;
            context.player.GetParasiteController().SetParasite(null);

            context.player.GetCollisionController().CalculateNormalCollision();

            context.player.GetShotController().SetShotPoint(context.player.GetShotController().GetShotPoint());
            parasitePressed = false;
            gainHealth = false;
        }

        private void OnDestroy()
        {
            if (parasiteEnemy != null)
                parasiteEnemy.GetToleranceCtrl().OnMaxTolleranceBar -= HandleOnMaxTolleranceBar;
            context.player.OnEnemyCollision -= HandleOnEnemyCollision;
            context.player.OnDamageableCollision -= HandleOnDamageableCollision;
            context.player.OnPlayerImmunityEnd -= HandlePlayerImmunityEnd;
            PlayerInputManager.OnParasitePressed -= HandleOnPlayerParasitePressed;
        }
    }
}
