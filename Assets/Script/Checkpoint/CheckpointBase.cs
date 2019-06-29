using UnityEngine;
using System;

public abstract class CheckpointBase : MonoBehaviour
{
    #region Actions
    public Action<CheckpointBase> ActivateCheckpoint;
    #endregion

    public abstract void Init();

    public virtual Vector3 GetPosition()
    {
        return transform.position;
    }

    public virtual void Enable()
    {

    }

    public virtual void Disable()
    {

    }
}
