using UnityEngine;
using System.Collections;

public class ColliderButton : ButtonBase
{
    private bool isActive = true;

    #region API
    public override void Init()
    {
        Setup();
    }

    public override void Setup()
    {
        isActive = true;
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
            Activate();
        }
    }
}
