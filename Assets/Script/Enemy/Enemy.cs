using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine.EnemySM;

[RequireComponent(typeof(Animator))]
public class Enemy : MonoBehaviour {

    [Header("Roaming Settings")]
    [SerializeField]
    private Transform leftLimit;
    private Transform rightLimit;

    [Header("Stun Settings")]
    [SerializeField]
    private int stunDuration;

    [Header("Death Settings")]
    [SerializeField]
    private int deathDuration;

    [Header("Other Settings")]
    [SerializeField]
    GameObject graphics;

    /// <summary>
    /// Reference to enemy's State Machine
    /// </summary>
    private EnemySMController enemySM;

    #region API
    /// <summary>
    /// Initialize Script
    /// </summary>
    public void Init() {

        // Initialize Enemy State Machine
        enemySM = GetComponent<EnemySMController>();
        enemySM.Init(this);

    }

    #region Getters
    /// <summary>
    /// Get left limit
    /// </summary>
    /// <returns>Trasform of left Limit Object</returns>
    public Transform LeftLimit
    {
        get { return leftLimit; }
    }

    /// <summary>
    /// Get right limit
    /// </summary>
    /// <returns>Trasform of right Limit Object</returns>
    public Transform RightLimit
    {
        get { return rightLimit; }
    }

    /// <summary>
    /// Get stun duration
    /// </summary>
    public int StunDuration
    {
        get { return stunDuration; }
    }

    /// <summary>
    /// Get Enemy state machine
    /// </summary>
    public EnemySMController EnemySM
    {
        get { return enemySM; }
    }

    /// <summary>
    /// Get Death Duration
    /// </summary>
    public int DeathDuration
    {
        get { return deathDuration; }
    }

    #endregion
    #endregion

}
