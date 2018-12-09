using UnityEngine;
using UnityEditor;

namespace StateMachine.EnemySM
{
    public class EnemySMController : StateMachineBase
    {
        #region Delegates
        public delegate void ChangeStateEvents();
        public ChangeStateEvents GoToStun;
        public ChangeStateEvents GoTnDeath;

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

            context = new EnemySMContext(enemy, EnemyEndStunCallback, EnemyRespawnCallback);

            foreach (StateMachineBehaviour state in enemySM.GetBehaviours<StateMachineBehaviour>())
            {
                IState newstate = state as IState;
                if (newstate != null)
                    newstate.Setup(context);
            }

            GoToStun += HandleEnemyStun;
            GoToParasite += HandleEnemyParasite;

            enemySM.SetTrigger("StartSM");
        }
        #endregion

        #region Handler
        private void HandleEnemyStun()
        {
            enemySM.SetTrigger("GoToStun");
        }

        private void HandleEnemyParasite(Player _player)
        {
            context.player = _player;
            enemySM.SetTrigger("GoToParasite");
        }
        #endregion

        #region Callbacks
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
        /// Callback per il respawn del nemico
        /// </summary>
        private void EnemyRespawnCallback()
        {
            enemySM.SetTrigger("GoToRoaming");
        }
        #endregion
    }

    public class EnemySMContext : IStateMachineContext
    {
        #region Delegates
        public delegate void EnemyCallbacks();
        public EnemyCallbacks EndStunCallback;
        public EnemyCallbacks RespawnCallback;
        #endregion

        public Player player;
        public IEnemy enemy;

        public EnemySMContext(IEnemy _enemy, EnemyCallbacks _endStunCallback, EnemyCallbacks _respawnCallback)
        {
            enemy = _enemy;
            EndStunCallback = _endStunCallback;
            RespawnCallback = _respawnCallback;
        }
    }
}
