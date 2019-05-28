using UnityEngine;
using System.Collections;
using System;

public class DoorButton : ButtonBase
{
    #region API
    public override void Init()
    {
        //ActivateEvent += HandleActivate;

        Setup();
    }

    public override void Setup()
    {
        
    }

    public override void Activate()
    {
        foreach (GameObject _current in targets)
        {
            IActivable _temp = _current.GetComponent<IActivable>();
            if (_temp != null)
                _temp.Activate();
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
