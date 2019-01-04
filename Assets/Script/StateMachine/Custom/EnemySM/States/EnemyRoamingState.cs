using UnityEngine;
using System.Collections;

namespace StateMachine.EnemySM
{

    public class EnemyRoamingState : EnemySMStateBase
    {
        private bool canMove = false;
        private Transform _playerTransform;
        private EnemyViewController viewCtrl;

        /// <summary>
        /// Function that activate on state enter
        /// </summary>
        public override void Enter()
        {
            //Giusto per notare il cambio di stato nella build (da togliere)
            context.enemy.gameObject.GetComponentInChildren<MeshRenderer>().material.color = Color.red;
            Debug.Log("Enter Roaming State");

            _playerTransform = LevelManager.singleton.GetPlayerTransform();
            viewCtrl = context.enemy.GetViewCtrl();

            context.enemy.SetPath();
            canMove = true;
        }

        /// <summary>
        /// Behaviour during Update
        /// </summary>
        public override void Tick()
        {
            if (canMove)
            {
                context.enemy.MoveRoaming();

                if (viewCtrl.CheckPlayerDistance(_playerTransform.position))
                {
                    if (viewCtrl.CanSeePlayer(_playerTransform.position))
                    {
                        context.enemy.Alert();
                    }
                }
            }
        }

        /// <summary>
        /// Function that activate on state exit
        /// </summary>
        public override void Exit()
        {
            Debug.Log("Leaving Roaming State");
            canMove = false;
        }
    }

}

