using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine.PlayerSM
{
    public class PlayerNormalState : PlayerSMStateBase
    {
        public override void Enter()
        {
            PlayerInputManager.OnParasitePressed += HandleOnPlayerParasitePressed;
            context.player.OnDamageableCollision += OnDamageableCollision;
            if (context.player.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                context.player.OnEnemyCollision += OnEnemyCollision;
                immunity = false;
                loseHealth = true;
            }
            else
            {
                context.player.OnPlayerImmunityEnd += PlayerImmunityEnd;
                immunity = true;
                loseHealth = false;
            }

            context.player.GetMovementController().SetCanMove(true);
            parasitePressed = false;            
        }
       
        public override void Tick()
        {
            if (loseHealth && !parasitePressed)
                context.player.GetHealthController().LoseHealthOverTime();
        }

        bool parasitePressed;
        private void HandleOnPlayerParasitePressed()
        {
            if (parasitePressed)
                return;

            IControllable e = context.player.GetParasiteController().CheckParasite();
            if (e != null)
            {
                parasitePressed = true;
                context.player.GetMovementController().SetCanMove(false);
                switch (e.GetControllableType())
                {
                    case ControllableType.Enemy:                        
                        context.player.StartParasiteEnemyCoroutine(e as IEnemy);
                        break;
                    case ControllableType.Platform:
                        context.player.StartParasitePlatformCoroutine(e as LaunchingPlatform);
                        break;
                }
            }
            else
                Debug.Log("Non ci sono nemici stunnati nelle vicinanze");
        }

        #region Enemy/Damageable Collision
        bool loseHealth;
        bool immunity;
        private void OnEnemyCollision(IEnemy _enemy)
        {
            context.player.OnEnemyCollision -= OnEnemyCollision;
            context.player.GetHealthController().DamageHit(_enemy.GetDamage());
            context.player.OnPlayerImmunityEnd += PlayerImmunityEnd;
            context.player.StartImmunityCoroutine(context.player.GetCollisionController().GetImmunityDuration());
            loseHealth = false;
            immunity = true;
        }

        private void OnDamageableCollision(IDamageable _damageable)
        {
            context.player.OnEnemyCollision -= OnEnemyCollision;

            switch (_damageable.DamageableType)
            {
                case DamageableType.Spike:
                    if (immunity)
                        return;
                    context.player.GetHealthController().DamageHit((_damageable as Spike).GetDamage());
                    context.player.OnPlayerImmunityEnd += PlayerImmunityEnd;
                    context.player.StartImmunityCoroutine(context.player.GetCollisionController().GetImmunityDuration());
                    break;
                case DamageableType.Acid:
                    context.player.OnDamageableCollision -= OnDamageableCollision;
                    context.player.StartDeathCoroutine();
                    break;
                default:
                    break;
            }

            loseHealth = false;
            immunity = true;
        }

        private void PlayerImmunityEnd()
        {
            loseHealth = true;
            immunity = false;
            context.player.OnEnemyCollision += OnEnemyCollision;
            context.player.OnPlayerImmunityEnd -= PlayerImmunityEnd;
        }
        #endregion

        public override void Exit()
        {
            parasitePressed = false;
            loseHealth = false;
            context.player.OnPlayerImmunityEnd -= PlayerImmunityEnd;
            context.player.OnDamageableCollision -= OnDamageableCollision;
            context.player.OnEnemyCollision -= OnEnemyCollision;
            PlayerInputManager.OnParasitePressed -= HandleOnPlayerParasitePressed;
        }

        private void OnDestroy()
        {
            context.player.OnPlayerImmunityEnd -= PlayerImmunityEnd;
            context.player.OnDamageableCollision -= OnDamageableCollision;
            context.player.OnEnemyCollision -= OnEnemyCollision;
            PlayerInputManager.OnParasitePressed -= HandleOnPlayerParasitePressed;
        }
    }
}
