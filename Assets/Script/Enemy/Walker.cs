using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine.EnemySM;

[RequireComponent(typeof(Animator))]
public class Walker : EnemyBase
{
    [Header("Walker Settings")]
    [SerializeField]
    protected Transform leftLimit;
    [SerializeField]
    protected Transform rightLimit;

    #region API
    public override void Move()
    {
        Debug.Log("Mi sto muovendo");
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
    #endregion
    #endregion

}
