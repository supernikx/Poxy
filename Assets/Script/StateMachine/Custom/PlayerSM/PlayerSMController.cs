using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine.PlayerSM
{
    public class PlayerSMController : StateMachineBase
    {
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

            playerSM.SetTrigger("StartSM");
        }
    }

    public class PlayerSMContext : IStateMachineContext
    {
        public Player player;

        public PlayerSMContext(Player _player)
        {
            player = _player;
        }
    }
}
