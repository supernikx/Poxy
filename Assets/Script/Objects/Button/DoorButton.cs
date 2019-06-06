using UnityEngine;
using System.Collections;
using System;

public class DoorButton : ButtonBase
{
    #region API
    public override void Init()
    {
        //ActivateEvent += HandleActivate;
        base.Init();
        Setup();
    }

    public override void Setup()
    {
        base.Setup();
    }

    public override void Activate()
    {
        base.Activate();
        foreach (GameObject _current in targets)
        {
            IActivable _temp = _current.GetComponent<IActivable>();
            if (_temp != null)
                _temp.Activate();
        }
    }
    #endregion
}
