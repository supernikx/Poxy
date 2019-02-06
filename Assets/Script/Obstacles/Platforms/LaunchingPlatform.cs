using UnityEngine;
using System.Collections;

public class LaunchingPlatform : MonoBehaviour, IPlatform, IControllable
{
    private ControllableType controllableType = ControllableType.Platform;

    public void Init()
    {

    }

    #region Getters
    public ControllableType GetControllableType()
    {
        return controllableType;
    }
    #endregion
}
