using UnityEngine;
using System.Collections;

public class StandardCheckpoint : CheckpointBase
{
    CheckpointAnimationController animMng;
    GeneralSoundController soundCtrl;

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
        soundCtrl = GetComponentInChildren<GeneralSoundController>();
        animMng = GetComponentInChildren<CheckpointAnimationController>();
        animMng.Init();
    }

    public override void Enable()
    {
        animMng.Enable(true);
        soundCtrl.PlayClip();
    }

    public override void Disable()
    {
        animMng.Enable(false);
    }
    #endregion
}
