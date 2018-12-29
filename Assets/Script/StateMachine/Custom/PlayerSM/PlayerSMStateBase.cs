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

        #region Joystick Axis
        protected bool ltWasDown;
        protected bool CheckJoystickLTAxis()
        {
            if (Input.GetAxis("LT") > 0)
            {
                if (!ltWasDown)
                {
                    ltWasDown = true;
                    return true;
                }
                return false;
            }
            else
            {
                ltWasDown = false;
                return false;
            }
        }

        bool rtWasDown;
        protected bool CheckJoystickRTAxis()
        {
            if (Input.GetAxis("RT") > 0)
            {
                if (!rtWasDown)
                {
                    rtWasDown = true;
                    return true;
                }
                return false;
            }
            else
            {
                rtWasDown = false;
                return false;
            }
        }
        #endregion
    }
}
