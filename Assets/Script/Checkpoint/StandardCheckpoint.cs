using UnityEngine;
using System.Collections;

public class StandardCheckpoint : CheckpointBase
{
    [Header("Standard Checkpoint Settings")]
    [SerializeField]
    private float range;

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
        GetComponent<SphereCollider>().radius = range;
        animMng = GetComponentInChildren<CheckpointAnimationController>();
        animMng.Init();
    }

    public override CheckpointAnimationController GetCheckpointAnimatorManager()
    {
        return animMng;
    }
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
