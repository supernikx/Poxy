using UnityEngine;
using System.Collections;

namespace StateMachine.EnemySM {

    public class EnemySMStateBase : StateBase
    {
        /// <summary>
        /// Reference to the Enemy Object
        /// </summary>
        protected EnemySMContext context;

        /// <summary>
        /// Setup of this State. Set the context of the State
        /// </summary>
        /// <param name="_context">Reference to the context of this State</param>
        /// <returns>This Instance</returns>
        public override void Setup(IStateMachineContext _context)
        {
            context = _context as EnemySMContext;
        }
    }

}

