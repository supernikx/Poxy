using UnityEngine;
using DG.Tweening;
using System.Collections;

namespace StateMachine.EnemySM
{

    public class EnemyRoamingState : EnemySMStateBase
    {
        private IEnemy enemy;
        private EnemyViewController viewCtrl;
        private Vector3 movementVector;
        private float pathLenght;
        private float pathTraveled;
        private float targetAngle;
        private bool movementBlocked;

        /// <summary>
        /// Function that activate on state enter
        /// </summary>
        public override void Enter()
        {
            enemy = context.enemy;
            viewCtrl = enemy.GetViewCtrl();
            RoamingSetup();
        }

        private void RoamingSetup()
        {
            enemy.EnemyRoamingState();
            movementVector = Vector3.zero;
            targetAngle = enemy.gameObject.transform.localEulerAngles.y;
            pathLenght = enemy.GetPathLenght();
            pathTraveled = 0f;
            movementBlocked = false;

            if (enemy.gameObject.transform.localEulerAngles.y != targetAngle)
            {
                Vector3 rotationVector = Vector3.zero;
                rotationVector.y = targetAngle;
                enemy.gameObject.transform.eulerAngles = rotationVector;
            }
        }

        /// <summary>
        /// Coroutine che gestisce il movimento del nemico
        /// </summary>
        /// <returns></returns>
        private void MoveRoaming()
        {
            if (enemy.GetCollisionCtrl().GetCollisionInfo().StickyCollision())
                return;

            if (enemy.GetCollisionCtrl().GetCollisionInfo().HorizontalCollision())
                pathTraveled = pathLenght;

            if (pathTraveled >= pathLenght - 0.1f)
            {
                pathTraveled = 0f;
                Vector3 rotationVector = Vector3.zero;
                if (enemy.gameObject.transform.rotation.y == 0)
                    rotationVector.y = 180f;
                targetAngle = rotationVector.y;
                enemy.gameObject.transform.eulerAngles = rotationVector;
            }

            //Movimento Nemico                
            movementVector.x = enemy.GetMovementSpeed();
            Vector3 distanceTraveled = enemy.GetMovementCtrl().MovementCheck(movementVector);

            enemy.GetAnimationController().MovementAnimation(distanceTraveled);
            pathTraveled += distanceTraveled.x;
        }


        /// <summary>
        /// Behaviour during Update
        /// </summary>
        public override void Tick()
        {
            Transform playerTransform = viewCtrl.FindPlayer();
            if (playerTransform != null)
            {
                if (viewCtrl.CanSeePlayer(playerTransform.position))
                    context.AlertCallback();
                else
                    MoveRoaming();
            }
            else
                MoveRoaming();
        }
    }
}

