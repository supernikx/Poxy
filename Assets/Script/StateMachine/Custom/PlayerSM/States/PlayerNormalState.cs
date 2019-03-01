using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine.PlayerSM
{
    public class PlayerNormalState : PlayerSMStateBase
    {
        public override void Enter()
        {
            context.player.OnEnemyCollision += OnEnemyCollision;
            context.player.OnDemageableCollision += OnDamageableCollision;
            context.player.GetShotController().ChangeShotType();
            context.player.GetMovementController().SetCanMove(true);
            parasitePressed = false;
            loseHealth = true;
        }

        public override void Exit()
        {
            parasitePressed = false;
            loseHealth = false;
            context.player.OnPlayerImmunityEnd -= PlayerImmunityEnd;
            context.player.OnDemageableCollision -= OnDamageableCollision;
            context.player.OnEnemyCollision -= OnEnemyCollision;
        }

        bool parasitePressed;
        public override void Tick()
        {
            if (Input.GetButtonDown("Parasite") && !parasitePressed)
            {
                IControllable e = context.player.GetParasiteController().CheckParasite();
                if (e != null)
                {
                    switch (e.GetControllableType())
                    {
                        case ControllableType.Enemy:
                            parasitePressed = true;
                            context.player.GetMovementController().SetCanMove(false);
                            context.player.StartParasiteEnemyCoroutine(e as IEnemy);
                            break;
                        case ControllableType.Platform:
                            parasitePressed = true;
                            context.player.GetMovementController().SetCanMove(false);
                            context.player.StartParasitePlatformCoroutine(e as LaunchingPlatform);  
                            break;
                        default:
                            break;
                    }
                    
                }
                else
                    Debug.Log("Non ci sono nemici stunnati nelle vicinanze");
            }

            if (loseHealth && !parasitePressed)
                context.player.GetHealthController().LoseHealthOverTime();
        }

        #region Enemy Collision
        bool loseHealth;
        private void OnEnemyCollision(IEnemy _enemy)
        {
            loseHealth = false;
            context.player.OnDemageableCollision -= OnDamageableCollision;
            context.player.OnEnemyCollision -= OnEnemyCollision;
            context.player.GetHealthController().DamageHit(_enemy.GetDamage());
            context.player.OnPlayerImmunityEnd += PlayerImmunityEnd;
            context.player.StartImmunityCoroutine(context.player.GetCollisionController().GetImmunityDuration());
        }

        private void OnDamageableCollision(IDamageable _damageable)
        {
            loseHealth = false;
            context.player.OnDemageableCollision -= OnDamageableCollision;
            context.player.OnEnemyCollision -= OnEnemyCollision;

            switch (_damageable.DamageableType)
            {
                case DamageableType.Spike:                    
                    context.player.GetHealthController().DamageHit((_damageable as Spike).GetDamage());
                    context.player.OnPlayerImmunityEnd += PlayerImmunityEnd;
                    context.player.StartImmunityCoroutine(context.player.GetCollisionController().GetImmunityDuration());
                    break;
                case DamageableType.Acid:
                    context.player.StartDeathCoroutine();
                    break;
                default:
                    break;
            }
        }

        private void PlayerImmunityEnd()
        {
            loseHealth = true;
            context.player.OnEnemyCollision += OnEnemyCollision;
            context.player.OnDemageableCollision += OnDamageableCollision;
            context.player.OnPlayerImmunityEnd -= PlayerImmunityEnd;
        }
        #endregion
    }
}
