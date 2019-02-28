﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyButton : ButtonBase
{
    [Header("Enemies to Kill")]
    [SerializeField]
    private List<EnemyBase> enemiesToKill = new List<EnemyBase>();

    private List<EnemyBase> killed = new List<EnemyBase>();

    #region API
    public override void Init()
    {
        Debug.Log("init");
        foreach (EnemyBase _current in enemiesToKill)
        {
            _current.OnEnemyDeath += HandleOnEnemyDeath;
        }
    }

    public override void Activate()
    {
        foreach (DoorBase _current in targets)
        {
            _current.Activate();
        }
    }
    #endregion

    #region Handlers
    private void HandleOnEnemyDeath(EnemyBase _enemy)
    {
        Debug.Log("call");
        if (enemiesToKill.Contains(_enemy) && !killed.Contains(_enemy))
        {
            killed.Add(_enemy);
            _enemy.OnEnemyDeath -= HandleOnEnemyDeath;
        }

        if (enemiesToKill.Count == killed.Count)
            Activate();
    }
    #endregion
}
