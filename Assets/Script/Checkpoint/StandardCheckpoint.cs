using UnityEngine;
using System.Collections;

public class StandardCheckpoint : CheckpointBase
{
    [Header("Standard Checkpoint Settings")]
    [SerializeField]
    private float range;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            ActivateCheckpoint(this);
        }
    }

    #region API
    public override void Init()
    {
        GetComponent<SphereCollider>().radius = range;
    }
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
