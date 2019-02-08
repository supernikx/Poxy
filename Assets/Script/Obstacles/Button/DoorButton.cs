using UnityEngine;
using System.Collections;
using System;

public class DoorButton : ButtonBase
{
    #region API
    public override void Init()
    {
        //ActivateEvent += HandleActivate;
    }

    public override void Activate()
    {
        foreach (DoorBase _current in targets)
        {
            _current.Activate();
        }
    }
    #endregion

    #region Handlers
    /*
    private void HandleActivate(object sender, EventArgs e)
    {
        Debug.Log("Activate");
    }
    */
    #endregion
}
