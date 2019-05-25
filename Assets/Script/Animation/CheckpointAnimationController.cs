using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointAnimationController : MonoBehaviour
{
    Animator anim;
    bool enable;

    public void Init()
    {
        anim = GetComponentInChildren<Animator>();
        enable = false;
    }

    public void Enable(bool _enable)
    {
        if (enable && !_enable)
        {
            anim.SetTrigger("Disable");
            enable = _enable;
        }
        else if (!enable && _enable)
        {
            anim.SetTrigger("Enable");
            enable = _enable;
        }
    }
}
