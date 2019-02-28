using UnityEngine;
using System.Collections;
using System;

public class SimpleDoor : DoorBase
{
    private bool isActive;

    #region API
    public override void Init()
    {
        //ActivateEvent += HandleActivate;
        Setup();
    }

    public override void Setup()
    {
        isActive = true;
        gameObject.SetActive(isActive);
    }

    public override void Activate()
    {
        isActive = !isActive;
        this.gameObject.SetActive(isActive);
    }
    #endregion

    #region Handlers
    /*
    private void HandleActivate(object sender, EventArgs e)
    {
        Debug.Log("Activate");
    }
    */
    #endregion
}
