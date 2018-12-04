using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine.PlayerSM
{
    public class PlayerSMStateBase : StateBase
    {
        protected PlayerSMContext context;
        public override IState Setup(IStateMachineContext _context)
        {
            context = _context as PlayerSMContext;
            return this;
        }
    }
}
