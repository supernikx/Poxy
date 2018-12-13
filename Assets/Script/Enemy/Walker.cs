using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine.EnemySM;
using DG.Tweening;

[RequireComponent(typeof(Animator))]
public class Walker : EnemyBase
{

    #region API
    public override void Move()
    {
        transform.DOPath(GetWaypoints(), 5).SetOptions(true).SetLoops(-1).SetEase(Ease.Linear);
    }

    public override void Stop()
    {
        DOTween.Kill(this.transform, false);
    }

    #region Getters
    /// <summary>
    /// Get stun duration
    /// </summary>
    public override int GetStunDuration()
    {
        return stunDuration;
    }

    /// <summary>
    /// Get Death Duration
    /// </summary>
    public override int GetDeathDuration()
    {
        return deathDuration;
    }

    /// <summary>
    /// Get Graphics Reference
    /// </summary>
    public override GameObject GetGraphics()
    {
        return graphics;
    }

    /// <summary>
    /// Get Collider Reference
    /// </summary>
    public override BoxCollider GetCollider()
    {
        return GetComponent<BoxCollider>();
    }
    #endregion
    #endregion

}
