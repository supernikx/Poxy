﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace StateMachine.GameSM
{
    public class GameSMController : StateMachineBase
    {
        #region Delegates
        public delegate void GameSMEvents();
        public GameSMEvents GoToLevelSetup;
        #endregion

        protected Animator gameSM;
        protected GameSMContext context;
        protected GameManager gameManager;

        public void Init(GameManager _gameManager)
        {
            gameSM = GetComponent<Animator>();

            gameManager = _gameManager;
            context = new GameSMContext(gameManager)
            {
                OnLevelSetupCallback = HandleOnLevelSetup
            };

            foreach (StateMachineBehaviour state in gameSM.GetBehaviours<StateMachineBehaviour>())
            {
                IState newstate = state as IState;
                if (newstate != null)
                    newstate.Setup(context);
            }

            GoToLevelSetup += HandleLevelSetupState;

            gameSM.SetTrigger("StartSM");
        }

        #region Handle
        /// <summary>
        /// Handle all'evento GoToLevelSetup
        /// </summary>
        public void HandleLevelSetupState()
        {
            gameSM.SetTrigger("GoToLevelSetup");
        }

        /// <summary>
        /// Handle alla callback OnLevelSetupCallback
        /// </summary>
        public void HandleOnLevelSetup()
        {
            gameSM.SetTrigger("GoToGameplay");
        }
        #endregion
    }

    public class GameSMContext : IStateMachineContext
    {
        public Action OnLevelSetupCallback;
        public GameManager gameManager;

        public GameSMContext(GameManager _gameManager)
        {
            gameManager = _gameManager;
        }
    }
}
