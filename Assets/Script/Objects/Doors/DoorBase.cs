using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public abstract class DoorBase : MonoBehaviour, IDoor
{
    /*
    #region Events
    public event EventHandler ActivateEvent;

    object objectLock = new System.Object();

    event EventHandler IDoor.ActivateEvent
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
    #endregion
    */

    #region API
    public abstract void Init();
    public abstract void Activate();
    #endregion
}
