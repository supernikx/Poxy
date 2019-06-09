using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySoundController : SoundControllerBase
{
    [Header("General Sound Settings")]
    [SerializeField]
    private AudioClipStruct alert;

    [Header("Life Sound Settings")]
    [SerializeField]
    private AudioClipStruct damageHit;
    [SerializeField]
    private AudioClipStruct death;

    EnemyBase enemy;

    public void Setup(EnemyBase _enemy)
    {
        enemy = _enemy;
    }

    public override void Init()
    {
        base.Init();

        EnemyManager.OnEnemyDeath += HandleOnEnemyDeath;
        enemy.OnEnemyHit += HandleEnemyHit;
        enemy.OnEnemyAlert += HandleEnemyAlert;
    }

    #region Hanlders
    private void HandleEnemyAlert()
    {
        PlayAudioClip(alert);
    }

    private void HandleEnemyHit()
    {
        PlayAudioClip(damageHit);
    }

    private void HandleOnEnemyDeath(IEnemy e)
    {
        if (e == (enemy as IEnemy))
            PlayAudioClip(death);
    }
    #endregion

    private void OnDisable()
    {
        EnemyManager.OnEnemyDeath -= HandleOnEnemyDeath;
        enemy.OnEnemyHit -= HandleEnemyHit;
        enemy.OnEnemyAlert -= HandleEnemyAlert;
    }
}
