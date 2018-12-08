using UnityEngine;
using UnityEditor;

namespace StateMachine.EnemySM
{
    public class EnemySMController : StateMachineBase
    {
        protected Animator enemySM;
        protected EnemySMContext context;

        #region API
        public void Init(Enemy _enemy)
        {
            enemySM = GetComponent<Animator>();

            context = new EnemySMContext(_enemy);

            foreach (StateMachineBehaviour state in enemySM.GetBehaviours<StateMachineBehaviour>())
            {
                IState newstate = state as IState;
                if (newstate != null)
                    newstate.Setup(context);
            }

            enemySM.SetTrigger("StartSM");
        }

        /// <summary>
        /// Change Current State of State Machine
        /// </summary>
        /// <param name="_trigger">Name of the Trigger that change State</param>
        public void ChangeState(string _trigger)
        {
            enemySM.SetTrigger(_trigger);
        }

        #endregion
    }

    public class EnemySMContext : IStateMachineContext
    {
        public Enemy enemy;

        public EnemySMContext(Enemy _enemy)
        {
            enemy = _enemy;
        }
    }
}
