using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine.EnemySM;

[RequireComponent(typeof(Animator))]
public class Walker : GroundEnemy {
    
    #region API
    /// <summary>
    /// Initialize Script
    /// </summary>
    public override void Init() {

        // Initialize Enemy State Machine
        enemySM = GetComponent<EnemySMController>();
        enemySM.Init(this);

    }

    #region Getters
    /// <summary>
    /// Get left limit
    /// </summary>
    /// <returns>Trasform of left Limit Object</returns>
    public override Transform GetLeftLimit()
    {
        return leftLimit;
    }

    /// <summary>
    /// Get right limit
    /// </summary>
    /// <returns>Trasform of right Limit Object</returns>
    public override Transform GetRightLimit()
    {
        return rightLimit;
    }

    /// <summary>
    /// Get stun duration
    /// </summary>
    public override int GetStunDuration()
    {
        return stunDuration;
    }

    /// <summary>
    /// Get Enemy state machine
    /// </summary>
    public override EnemySMController GetEnemySM()
    {
        return enemySM;
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
