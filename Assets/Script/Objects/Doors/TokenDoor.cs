using UnityEngine;
using System.Collections;

public class TokenDoor : DoorBase
{
    private bool isActive;

    #region API
    public override void Init()
    {
        isActive = true;
    }

    public override void Activate()
    {
        isActive = !isActive;
        this.gameObject.SetActive(isActive);
    }
    #endregion
}
