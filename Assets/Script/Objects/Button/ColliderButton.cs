using UnityEngine;
using System.Collections;

public class ColliderButton : ButtonBase
{
    private bool active = true;

    #region API
    public override void Init()
    {
        active = true;
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
        if (active && collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            active = false;
            Activate();
        }
    }
}
