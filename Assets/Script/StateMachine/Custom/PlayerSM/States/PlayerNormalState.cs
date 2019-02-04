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
            context.player.GetShotController().ChangeShotType();
            context.player.GetMovementController().SetCanMove(true);
            parasitePressed = false;
            loseHealth = true;
        }

        public override void Exit()
        {
            parasitePressed = false;
            loseHealth = false;
            context.player.OnEnemyCollision -= OnEnemyCollision;
        }

        bool parasitePressed;
        public override void Tick()
        {
            if (Input.GetButtonDown("Parasite") && !parasitePressed)
            {
                IEnemy e = context.player.GetParasiteController().CheckParasite();
                if (e != null)
                {
                    parasitePressed = true;
                    context.player.StartParasiteCoroutine(e);
                    context.player.GetMovementController().SetCanMove(false);
                }
                else
                    Debug.Log("Non ci sono nemici stunnati nelle vicinanze");
            }

            if (loseHealth)
                context.player.GetHealthController().LoseHealthOverTime();
        }

        #region Enemy Collision
        bool loseHealth;
        private void OnEnemyCollision(IEnemy _enemy)
        {
            loseHealth = false;
            context.player.OnEnemyCollision -= OnEnemyCollision;
            context.player.GetHealthController().DamageHit(_enemy.GetDamage());
            context.player.OnPlayerImmunityEnd += PlayerImmunityEnd;
            context.player.StartImmunityCoroutine(context.player.GetCollisionController().GetImmunityDuration());
        }

        private void PlayerImmunityEnd()
        {
            loseHealth = true;
            context.player.OnEnemyCollision += OnEnemyCollision;
            context.player.OnPlayerImmunityEnd -= PlayerImmunityEnd;
        }
        #endregion
    }
}
