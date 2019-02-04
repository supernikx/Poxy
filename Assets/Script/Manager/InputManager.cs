using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputManager
{
    private static bool ltWasDown;
    public static bool GetLTDown()
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

    private static bool rtWasDown;
    public static bool GetRTDown()
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

    public static bool GetRT()
    {
        if (Input.GetAxis("RT") > 0)
        {
            if (!rtWasDown)
            {
                rtWasDown = true;
            }
            return true;
        }
        else
        {
            rtWasDown = false;
            return false;
        }
    }
}
