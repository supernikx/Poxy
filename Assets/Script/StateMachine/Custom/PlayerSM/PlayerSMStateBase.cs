using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine.PlayerSM
{
    public class PlayerSMStateBase : StateBase
    {
        protected PlayerSMContext context;
        public override void Setup(IStateMachineContext _context)
        {
            context = _context as PlayerSMContext;
        }
    }
}
