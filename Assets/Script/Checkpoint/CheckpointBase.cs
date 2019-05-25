using UnityEngine;
using System.Collections;

public abstract class CheckpointBase : MonoBehaviour
{

    #region Delegates
    public delegate void ActivateCheckpointEvent(CheckpointBase _checkpoint);
    public ActivateCheckpointEvent ActivateCheckpoint;
    #endregion

    public abstract void Init();

    public virtual Vector3 GetPosition()
    {
        return transform.position;
    }

    public virtual CheckpointAnimationController GetCheckpointAnimatorManager()
    {
        return null;
    }
}
