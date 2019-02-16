using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public abstract class ButtonBase : MonoBehaviour, IButton
{
    /*
    #region Events
    public event EventHandler ActivateEvent;

    object objectLock = new System.Object();

    event EventHandler IButton.ActivateEvent
    {
        add
        {
            lock (objectLock)
            {
                ActivateEvent += value;
            }
        }
        remove
        {
            lock (objectLock)
            {
                ActivateEvent -= value;
            }
        }
    }

    public void Activate()
    {
        ActivateEvent?.Invoke(this, EventArgs.Empty);
    }
    #endregion*/

    [Header("Button Target")]
    [SerializeField]
    protected List<DoorBase> targets = new List<DoorBase>();

    public abstract void Init();
    public abstract void Activate();

}
