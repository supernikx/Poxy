using UnityEngine;
using System.Collections;
using System;

public class SimpleDoor : DoorBase
{
    [Header("Door options")]
    [SerializeField]
    private bool isActive;

    private bool currentState;

    #region API
    public override void Init()
    {
        //ActivateEvent += HandleActivate;
        Setup();
    }

    public override void Setup()
    {
        currentState = isActive;
        gameObject.SetActive(currentState);
    }

    public override void Activate()
    {
        currentState = !currentState;
        this.gameObject.SetActive(currentState);
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
