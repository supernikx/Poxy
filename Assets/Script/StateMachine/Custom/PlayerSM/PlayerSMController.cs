using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine.PlayerSM
{
    public class PlayerSMController : MonoBehaviour
    {
        #region Delegates
        public delegate void PlayerParasiteEvent(IEnemy _enemy);
        public PlayerParasiteEvent OnPlayerParaiste;

        public delegate void PlayerNormalevent();
        public PlayerNormalevent OnPlayerNormal;
        #endregion

        protected Animator playerSM;
        protected PlayerSMContext context;

        public void Init(Player _player)
        {
            playerSM = GetComponent<Animator>();

            context = new PlayerSMContext(_player);

            foreach (StateMachineBehaviour state in playerSM.GetBehaviours<StateMachineBehaviour>())
            {
                IState newstate = state as IState;
                if (newstate != null)
                    newstate.Setup(context);
            }

            OnPlayerParaiste += HandlePlayerParasite;
            OnPlayerNormal += HandlePlayerNormal;

            _player.OnPlayerDeath += HandlePlayerDeath;

            playerSM.SetTrigger("StartSM");
        }

        #region Handler
        /// <summary>
        /// Handler per mandare il player nello stato di parassita
        /// </summary>
        /// <param name="_enemy"></param>
        private void HandlePlayerParasite(IEnemy _enemy)
        {
            context.parasiteEnemy = _enemy;
            playerSM.SetTrigger("GoToParasite");
        }

        /// <summary>
        /// Handler per mandatare il player nello stato normale
        /// </summary>
        private void HandlePlayerNormal()
        {
            playerSM.SetTrigger("GoToNormal");
        }

        /// <summary>
        /// Handler per mandare il player nello stato di morte
        /// </summary>
        private void HandlePlayerDeath()
        {
            playerSM.SetTrigger("GoToDeath");
        }
        #endregion
    }

    public class PlayerSMContext : IStateMachineContext
    {
        public Player player;
        public IEnemy parasiteEnemy;

        public PlayerSMContext(Player _player)
        {
            player = _player;
        }
    }
}
