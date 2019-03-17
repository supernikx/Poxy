using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlatformBase : MonoBehaviour, IPlatform
{
    public abstract void Init();

    public virtual void MoveBehaviour()
    {
        return;
    }
}
