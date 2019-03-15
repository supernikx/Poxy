using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine.PlayerSM
{
    public class PlayerParasiteState : PlayerSMStateBase
    {
        private bool healthMax;
        private IEnemy parasiteEnemy;

        public override void Enter()
        {
            parasiteEnemy = context.parasite as IEnemy;

            parasiteEnemy.GetToleranceCtrl().OnMaxTolleranceBar += OnMaxTolleranceBar;            
            parasiteEnemy.gameObject.layer = context.player.gameObject.layer;
            if (context.player.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                context.player.OnEnemyCollision += OnEnemyCollision;
                context.player.OnDemageableCollision += OnDamageableCollision;
            }
            else
            {
                context.player.OnPlayerImmunityEnd += PlayerImmunityEnd;
            }       

            parasitePressed = false;
            healthMax = false;

            parasiteEnemy.gameObject.transform.parent = context.player.transform;
            parasiteEnemy.gameObject.transform.localPosition = Vector3.zero;
            parasiteEnemy.gameObject.transform.localRotation = Quaternion.identity;

            context.player.GetCollisionController().CalculateParasiteCollision(parasiteEnemy);
            context.player.GetMovementController().SetCanMove(true);
            context.player.GetShotController().SetShotPoint(parasiteEnemy.GetShotPoint());
            context.player.GetShotController().SetCanShootDamage(true);
        }

        bool parasitePressed;
        public override void Tick()
        {
            if (Input.GetButtonDown("Parasite") && !parasitePressed)
            {
                parasitePressed = true;
                context.player.StartNormalCoroutine();
            }

            if (Input.GetButtonDown("SwitchFire"))
            {
                //Cambio tipo di sparo
                context.player.GetShotController().ChangeShotType();
            }

            if (context.player.GetHealthController().GainHealthOverTime() && !healthMax && !parasitePressed)
            {
                healthMax = true;
                if (context.player.OnPlayerMaxHealth != null)
                    context.player.OnPlayerMaxHealth();
            }
        }

        private void OnMaxTolleranceBar()
        {
            context.player.StartNormalCoroutine();
        }

        #region EnemyCollision
        private void OnEnemyCollision(IEnemy _enemy)
        {
            context.player.OnEnemyCollision -= OnEnemyCollision;
            context.player.OnDemageableCollision -= OnDamageableCollision;

            parasiteEnemy.GetToleranceCtrl().AddTolerance(_enemy.GetDamage());
            context.player.OnPlayerImmunityEnd += PlayerImmunityEnd;
            parasiteEnemy.gameObject.layer = LayerMask.NameToLayer("PlayerImmunity");
            context.player.StartImmunityCoroutine(context.player.GetCollisionController().GetImmunityDuration());
        }

        private void OnDamageableCollision(IDamageable _damageable)
        {
            context.player.OnDemageableCollision -= OnDamageableCollision;
            context.player.OnEnemyCollision -= OnEnemyCollision;

            switch (_damageable.DamageableType)
            {
                case DamageableType.Spike:
                    parasiteEnemy.GetToleranceCtrl().AddTolerance((_damageable as Spike).GetDamage());
                    context.player.OnPlayerImmunityEnd += PlayerImmunityEnd;
                    parasiteEnemy.gameObject.layer = LayerMask.NameToLayer("PlayerImmunity");
                    context.player.StartImmunityCoroutine(context.player.GetCollisionController().GetImmunityDuration());
                    break;
                case DamageableType.Acid:
                    context.player.StartNormalCoroutine();
                    break;
                default:
                    break;
            }
        }

        private void PlayerImmunityEnd()
        {
            context.player.OnDemageableCollision += OnDamageableCollision;
            context.player.OnEnemyCollision += OnEnemyCollision;
            context.player.OnPlayerImmunityEnd -= PlayerImmunityEnd;
            parasiteEnemy.gameObject.layer = LayerMask.NameToLayer("Player");
        }
        #endregion

        public override void Exit()
        {
            context.player.OnEnemyCollision -= OnEnemyCollision;
            context.player.OnDemageableCollision -= OnDamageableCollision;
            context.player.OnPlayerImmunityEnd -= PlayerImmunityEnd;

            parasiteEnemy.GetToleranceCtrl().OnMaxTolleranceBar -= OnMaxTolleranceBar;
            context.player.GetParasiteController().SetParasite(null);

            context.player.GetCollisionController().CalculateNormalCollision();

            context.player.GetShotController().SetShotPoint(context.player.GetShotController().GetShotPoint());
            context.player.GetShotController().SetCanShootDamage(false);
            parasitePressed = false;
            healthMax = false;
        }
    }
}
