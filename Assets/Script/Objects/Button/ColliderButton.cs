using UnityEngine;
using System.Collections;

public class ColliderButton : ButtonBase
{
    private bool isActive = true;
    private Collider coll;

    #region API
    public override void Init()
    {
        coll = GetComponent<Collider>();
        Setup();
    }

    public override void Setup()
    {
        isActive = true;
        coll.enabled = isActive;
    }

    public override void Activate()
    {
        foreach (DoorBase _current in targets)
        {
            _current.Activate();
        }
    }
    #endregion

    private void OnTriggerEnter(Collider collision)
    {
        if (isActive && collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            isActive = false;
            coll.enabled = isActive;
            Activate();
        }
    }
}
