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
            context.enemy.SetCanTakeDamage(false);
            context.enemy.GetGraphics().ChangeTexture(TextureType.Parasite);
            context.player.OnPlayerMaxHealth += HandlePlayerMaxHealth;
            if (context.player.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                context.player.OnDamageableCollision += HandlePlayerDamageableCollision;
                context.player.OnEnemyCollision += HandlePlayerEnemyCollision;
                immunity = false;
            }
            else
            {
                context.player.OnPlayerImmunityEnd += HandlePlayerImmunityEnd;
                immunity = true;
            }
        }

        private void HandlePlayerMaxHealth()
        {
            tolleranceCtrl.SetActive(true);
        }

        #region Collision
        bool immunity;
        private void HandlePlayerImmunityEnd()
        {
            context.player.OnDamageableCollision += HandlePlayerDamageableCollision;
            context.player.OnEnemyCollision += HandlePlayerEnemyCollision;
            context.player.OnPlayerImmunityEnd -= HandlePlayerImmunityEnd;
            immunity = false;
        }

        private void HandlePlayerDamageableCollision(IDamageable damageable)
        {
            context.player.OnDamageableCollision -= HandlePlayerDamageableCollision;
            context.player.OnEnemyCollision -= HandlePlayerEnemyCollision;
            context.player.OnPlayerImmunityEnd += HandlePlayerImmunityEnd;
            immunity = true;
        }

        private void HandlePlayerEnemyCollision(IEnemy _enemy)
        {
            context.player.OnDamageableCollision -= HandlePlayerDamageableCollision;
            context.player.OnEnemyCollision -= HandlePlayerEnemyCollision;
            context.player.OnPlayerImmunityEnd += HandlePlayerImmunityEnd;
            immunity = true;
        }
        #endregion

        public override void Tick()
        {
            if (tolleranceCtrl.IsActive() && !immunity)
            {
                tolleranceCtrl.AddTolleranceOvertime();
            }

            if (tolleranceCtrl.CheckTolerance())
            {
                if (tolleranceCtrl.OnMaxTolleranceBar != null)
                    tolleranceCtrl.OnMaxTolleranceBar();
            }
        }

        public override void Exit()
        {
            context.player.OnPlayerMaxHealth -= HandlePlayerMaxHealth;
            context.player.OnDamageableCollision -= HandlePlayerDamageableCollision;
            context.player.OnEnemyCollision -= HandlePlayerEnemyCollision;
            context.player.OnPlayerImmunityEnd -= HandlePlayerImmunityEnd;

            context.player = null;

            tolleranceCtrl.SetActive(false);
            tolleranceCtrl = null;

            uiManager.GetGamePanel().EnableToleranceBar(false);

            context.enemy.GetGraphics().ChangeTexture(TextureType.Default);
            context.enemy.SetCanStun(true);
            context.enemy.SetCanTakeDamage(true);

            context.enemy.gameObject.transform.parent = context.enemy.GetEnemyDefaultParent();
            context.enemy.gameObject.layer = context.enemy.GetEnemyDefaultLayer();
        }

        private void OnDestroy()
        {
            if (context.player == null)
                return;
            context.player.OnPlayerMaxHealth -= HandlePlayerMaxHealth;
            context.player.OnDamageableCollision -= HandlePlayerDamageableCollision;
            context.player.OnEnemyCollision -= HandlePlayerEnemyCollision;
            context.player.OnPlayerImmunityEnd -= HandlePlayerImmunityEnd;
        }
    }
}
