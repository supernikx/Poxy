using UnityEngine;
using System.Collections;
using System;
using DG.Tweening;

public class SimpleDoor : DoorBase
{
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
        transform.DOKill();
        transform.position = startingPosition;
        gameObject.SetActive(true);
    }

    public override void Activate()
    {
        transform.DOMoveY(finalPosition.position.y, duration).OnComplete(() => gameObject.SetActive(false));
    }
    #endregion

    #region Handlers
    //private void HandleActivate(object sender, EventArgs e)
    //{
    //    Debug.Log("Activate");
    //}
    #endregion
}
