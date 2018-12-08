using UnityEngine;
using System.Collections;
using StateMachine.EnemySM;

public interface Enemy
{

    void Init();

    /// <summary>
    /// Get Stun Duration
    /// </summary
    int GetStunDuration();

    /// <summary>
    /// Get Enemy state machine
    /// </summary>
    EnemySMController GetEnemySM();

    /// <summary>
    /// Get Death Duration
    /// </summary>
    int GetDeathDuration();

    /// <summary>
    /// Get Graphics Reference
    /// </summary>
    GameObject GetGraphics();

}
