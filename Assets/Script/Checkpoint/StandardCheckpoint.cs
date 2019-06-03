using UnityEngine;
using System.Collections;

public class StandardCheckpoint : CheckpointBase
{
    CheckpointAnimationController animMng;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") || other.gameObject.layer == LayerMask.NameToLayer("PlayerImmunity"))
        {
            ActivateCheckpoint(this);
        }
    }

    #region API
    public override void Init()
    {
        animMng = GetComponentInChildren<CheckpointAnimationController>();
        animMng.Init();
    }

    public override CheckpointAnimationController GetCheckpointAnimatorManager()
    {
        return animMng;
    }
    #endregion
}
