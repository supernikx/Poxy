using UnityEngine;
using System.Collections;
using StateMachine.EnemySM;

public class GroundEnemy : MonoBehaviour, Enemy
{
    [Header("Roaming Settings")]
    [SerializeField]
    protected Transform leftLimit;
    [SerializeField]
    protected Transform rightLimit;

    [Header("Stun Settings")]
    [SerializeField]
    protected int stunDuration;

    [Header("Death Settings")]
    [SerializeField]
    protected int deathDuration;

    [Header("Other Settings")]
    [SerializeField]
    protected GameObject graphics;

    protected EnemySMController enemySM;

    #region API
    /// <summary>
    /// Initialize Script
    /// </summary>
    public virtual void Init()
    {

    }

    #region Getters
    /// <summary>
    /// Get left limit
    /// </summary>
    /// <returns>Trasform of left Limit Object</returns>
    public virtual Transform GetLeftLimit()
    {
        return null;
    }

    /// <summary>
    /// Get right limit
    /// </summary>
    /// <returns>Trasform of right Limit Object</returns>
    public virtual Transform GetRightLimit()
    {
        return null;
    }

    /// <summary>
    /// Get stun duration
    /// </summary>
    public virtual int GetStunDuration()
    {
        return 0;
    }

    /// <summary>
    /// Get Enemy state machine
    /// </summary>
    public virtual EnemySMController GetEnemySM()
    {
        return null;
    }

    /// <summary>
    /// Get Death Duration
    /// </summary>
    public virtual int GetDeathDuration()
    {
        return 0;
    }

    /// <summary>
    /// Get Graphics Reference
    /// </summary>
    public virtual GameObject GetGraphics()
    {
        return null;
    }
    #endregion
    #endregion
}
