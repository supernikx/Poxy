using UnityEngine;
using UnityEditor;
using System;

namespace StateMachine.EnemySM
{
    public class EnemySMController : StateMachineBase
    {
        #region Delegates
        public delegate void ChangeStateEvents();
        public ChangeStateEvents GoToStun;
        public ChangeStateEvents GoToDeath;
        public ChangeStateEvents GoToAfterParasite;

        public delegate void EnemyParasiteEvents(Player _player);
        public EnemyParasiteEvents GoToParasite;
        #endregion

        IEnemy enemy;
        private Animator enemySM;
        private EnemySMContext context;
        private EnemyManager enemyMng;

        #region API
        public void Init(IEnemy _enemy, EnemyManager _enemyMng)
        {
            enemyMng = _enemyMng;
            enemy = _enemy;
            enemySM = GetComponent<Animator>();

            context = new EnemySMContext(enemy, EnemyEndStunCallback, EnemyEndDeathCallback, EnemyEndParasiteCallback);

            foreach (StateMachineBehaviour state in enemySM.GetBehaviours<StateMachineBehaviour>())
            {
                IState newstate = state as IState;
                if (newstate != null)
                    newstate.Setup(context);
            }

            GoToStun += HandleEnemyStun;
            GoToDeath += HandleEnemyDeath;
            GoToParasite += HandleEnemyParasite;
            GoToAfterParasite += HandlerEnemyAfterParasite;

            enemySM.SetTrigger("StartSM");
        }
        #endregion

        #region Handler
        private void HandleEnemyStun()
        {
            enemySM.SetTrigger("GoToStun");
        }

        private void HandleEnemyDeath()
        {
            enemySM.SetTrigger("GoToDeath");
        }

        private void HandleEnemyParasite(Player _player)
        {
            context.player = _player;

            if (EnemyManager.OnEnemyEndStun != null)
                EnemyManager.OnEnemyEndStun(enemy);

            enemySM.SetTrigger("GoToParasite");
        }

        private void HandlerEnemyAfterParasite()
        {
            enemySM.SetTrigger("GoToAfterParasite");
        }
        #endregion

        #region Callbacks
        /// <summary>
        /// Callback per la fine dello stato di after parasite
        /// </summary>
        private void EnemyEndParasiteCallback()
        {
            enemySM.SetTrigger("GoToRoaming");
        }

        /// <summary>
        /// Callback per la fine dello stato di stun
        /// </summary>
        private void EnemyEndStunCallback()
        {
            if (EnemyManager.OnEnemyEndStun != null)
                EnemyManager.OnEnemyEndStun(enemy);

            enemySM.SetTrigger("GoToRoaming");
        }

        /// <summary>
        /// Callback per la fine dello stato di morte
        /// </summary>
        private void EnemyEndDeathCallback()
        {
            if (EnemyManager.OnEnemyEndDeath != null)
                EnemyManager.OnEnemyEndDeath(enemy);

            enemySM.SetTrigger("GoToRoaming");
        }
        #endregion
    }

    public class EnemySMContext : IStateMachineContext
    {
        public Action EndParasiteCallback;
        public Action EndStunCallback;
        public Action EndDeathCallback;
        public Player player;
        public IEnemy enemy;

        public EnemySMContext(IEnemy _enemy, Action _endStunCallback, Action _endDeathCallback, Action _endParasiteCallback)
        {
            enemy = _enemy;
            EndStunCallback = _endStunCallback;
            EndDeathCallback = _endDeathCallback;
            EndParasiteCallback = _endParasiteCallback;
        }
    }
}
