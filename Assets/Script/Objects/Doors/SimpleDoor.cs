using UnityEngine;
using System.Collections;
using System;
using DG.Tweening;

public class SimpleDoor : DoorBase
{
    [Header("Door options")]
    [SerializeField]
    private bool isActive;

    [Header("Animation Settings")]
    [SerializeField]
    private Transform finalPosition;
    [SerializeField]
    private float duration;

    private bool currentState;
    private Vector3 startingPosition;

    #region API
    public override void Init()
    {
        //ActivateEvent += HandleActivate;

        startingPosition = transform.position;
        Setup();
    }

    public override void Setup()
    {
        transform.position = startingPosition;
        currentState = isActive;
        gameObject.SetActive(currentState);
    }

    public override void Activate()
    {
        transform.DOMoveY(finalPosition.position.y, duration);
    }
    #endregion

    #region Handlers
    //private void HandleActivate(object sender, EventArgs e)
    //{
    //    Debug.Log("Activate");
    //}
    #endregion
}
