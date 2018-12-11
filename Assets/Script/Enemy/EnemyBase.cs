﻿using UnityEngine;
using System.Collections;
using StateMachine.EnemySM;

public abstract class EnemyBase : MonoBehaviour, IEnemy
{
    [Header("General Movement Settings")]
    protected float movementSpeed;

    [Header("Stun Settings")]
    [SerializeField]
    protected int stunDuration;

    [Header("Death Settings")]
    [SerializeField]
    protected int deathDuration;

    [Header("Other Settings")]
    [SerializeField]
    protected GameObject graphics;

    protected EnemyManager enemyMng;
    protected EnemySMController enemySM;

    #region API
    /// <summary>
    /// Initialize Script
    /// </summary>
    public virtual void Init(EnemyManager _enemyMng)
    {
        enemyMng = _enemyMng;

        // Initialize Enemy State Machine
        enemySM = GetComponent<EnemySMController>();
        if (enemySM != null)
            enemySM.Init(this, enemyMng);
    }

    /// <summary>
    /// Funzione che si ovvupa del movimento
    /// </summary>
    public abstract void Move();

    /// <summary>
    /// Funzione che manda il nemico in stato Stun
    /// </summary>
    public void Stun()
    {
        if (enemySM.GoToStun != null)
            enemySM.GoToStun();
    }

    /// <summary>
    /// Funzione che manda il nemico in stato Morte
    /// </summary>
    public void Die()
    {
        if (enemySM.GoToDeath != null)
            enemySM.GoToDeath();
    }

    /// <summary>
    /// Funzione che manda il nemico in stato parassita
    /// </summary>
    /// <param name="_player"></param>
    public void Parasite(Player _player)
    {
        if (enemySM.GoToParasite != null)
            enemySM.GoToParasite(_player);
    }

    #region Getters
    /// <summary>
    /// Get stun duration
    /// </summary>
    public virtual int GetStunDuration()
    {
        return stunDuration;
    }

    /// <summary>
    /// Get Death Duration
    /// </summary>
    public virtual int GetDeathDuration()
    {
        return deathDuration;
    }

    /// <summary>
    /// Get Graphics Reference
    /// </summary>
    public virtual GameObject GetGraphics()
    {
        return graphics;
    }

    /// <summary>
    /// Get Collider Reference
    /// </summary>
    public virtual BoxCollider GetCollider()
    {
        return GetComponent<BoxCollider>();
    }
    #endregion
    #endregion
}
