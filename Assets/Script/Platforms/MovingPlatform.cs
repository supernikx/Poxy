using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class MovingPlatform : Platform
{
    [Header("Movement Options")]
    [SerializeField]
    private float movingSpd;
    [SerializeField]
    private MovingDirection movingDirection;

    [Header("Waypoints")]
    [SerializeField]
    private Transform waypointParent;

    private Vector3[] path;

    private void SetPath()
    {
        path = new Vector3[waypointParent.childCount];
        for (int i = 0; i < path.Length; i++)
        {
            path[i] = waypointParent.GetChild(i).position;
        }
        waypointParent.gameObject.SetActive(false);
    }

    private void Move()
    {
        if (movingDirection == MovingDirection.horizontal)
        {
            transform.DOPath(path, movingSpd).SetEase(Ease.Linear).SetLoops(-1).SetSpeedBased().SetOptions(true, AxisConstraint.Y);
        }
        else if (movingDirection == MovingDirection.vertical)
        {
            transform.DOPath(path, movingSpd).SetEase(Ease.Linear).SetLoops(-1).SetSpeedBased().SetOptions(true, AxisConstraint.X);
        }
        else
        {
            transform.DOPath(path, movingSpd).SetEase(Ease.Linear).SetLoops(-1).SetSpeedBased().SetOptions(true);
        }
    }

    #region API
    public override void Init()
    {
        SetPath();
        Move();
    }
    #endregion
}

public enum MovingDirection
{
    horizontal,
    vertical,
    mixed
}
