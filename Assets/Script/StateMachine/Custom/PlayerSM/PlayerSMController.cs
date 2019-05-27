using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine.PlayerSM
{
    public class PlayerSMController : MonoBehaviour
    {
        #region Delegates
        public delegate void PlayerParasiteEvent(IControllable _enemy);
        public PlayerParasiteEvent OnPlayerEnemyParaiste;
        public PlayerParasiteEvent OnPlayerPlatformParaiste;

        public delegate void PlayerNormalevent();
        public PlayerNormalevent OnPlayerNormal;

        public Action GoToDeath;
        #endregion

        protected Animator playerSM;
        protected PlayerSMContext context;
        private Player player;
        
        public void Init(Player _player)
        {
            player = _player;
            playerSM = GetComponent<Animator>();

            context = new PlayerSMContext(player, LevelManager.instance.GetUIGameplayManager(), LevelManager.instance.GetCheckpointManager());

            foreach (StateMachineBehaviour state in playerSM.GetBehaviours<StateMachineBehaviour>())
            {
                IState newstate = state as IState;
                if (newstate != null)
                    newstate.Setup(context);
            }

            OnPlayerEnemyParaiste += HandlePlayerEnemyParasite;
            OnPlayerPlatformParaiste += HandlePlayerPlatformParasite;
            OnPlayerNormal += HandlePlayerNormal;
            GoToDeath += HandlePlayerDeath;

            playerSM.SetTrigger("StartSM");
        }

        #region Handler
        /// <summary>
        /// Handler per mandare il player nello stato di parassita
        /// </summary>
        /// <param name="_enemy"></param>
        private void HandlePlayerEnemyParasite(IControllable _enemy)
        {
            context.parasite = _enemy;
            playerSM.SetTrigger("GoToParasite");
        }

        private void HandlePlayerPlatformParasite(IControllable _platform)
        {
            context.parasite = _platform;
            playerSM.SetTrigger("GoToPlatform");
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

        private void OnDisable()
        {
            OnPlayerEnemyParaiste -= HandlePlayerEnemyParasite;
            OnPlayerPlatformParaiste -= HandlePlayerPlatformParasite;
            OnPlayerNormal -= HandlePlayerNormal;
            GoToDeath -= HandlePlayerDeath;
        }
    }

    public class PlayerSMContext : IStateMachineContext
    {
        public Player player;
        public CheckpointManager checkpointManager;
        public IControllable parasite;
        public UI_GameplayManager UIManager;

        public PlayerSMContext(Player _player, UI_GameplayManager _uiManager, CheckpointManager _checkpointManager)
        {
            player = _player;
            checkpointManager = _checkpointManager;
            UIManager = _uiManager;
        }
    }
}
