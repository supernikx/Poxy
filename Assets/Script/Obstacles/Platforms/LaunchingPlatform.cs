using UnityEngine;
using System.Collections;

public class LaunchingPlatform : Platform, IControllable
{
    [Header("Platform Specific Settings")]
    [SerializeField]
    private GameObject graphics;

    private ControllableType controllableType = ControllableType.Platform;

    //vuota per ora
    public override void Init()
    {

    }

    #region Getters
    public ControllableType GetControllableType()
    {
        return controllableType;
    }

    public GameObject GetGraphics()
    {
        return graphics;
    }
    #endregion
}
