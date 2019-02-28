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

    [Header("Button Options")]
    [SerializeField]
    protected ButtonTriggerType triggerType;
    [SerializeField]
    protected List<DoorBase> targets = new List<DoorBase>();

    #region Abstract
    public abstract void Init();
    public abstract void Setup();
    public abstract void Activate();
    #endregion

    #region Getters
    public virtual ButtonTriggerType GetTriggerType()
    {
        return triggerType;
    }
    #endregion
}
