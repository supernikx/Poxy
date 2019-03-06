using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVFXController : MonoBehaviour
{
    [Header("Hit VFX")]
    [SerializeField]
    private ParticleSystem hitVFX;
    [Header("Stun VFX")]
    [SerializeField]
    private ParticleSystem stunVfx;
    [Header("Death VFX")]
    [SerializeField]
    private ParticleSystem deathVFX;

    private EnemyBase enemy;

    public void Init(EnemyBase _enemy)
    {
        enemy = _enemy;
        enemy.OnEnemyHit += EnemyHitVFX;
        StopAllVFX();
    }

    /// <summary>
    /// Funzione che stoppa tutti i vfx
    /// </summary>
    public void StopAllVFX()
    {
        //Stun VFX
        stunEnable = false;
        stunVfx.Stop();
    }

    #region HitVFX
    /// <summary>
    /// Funzione che fa partire il VFX enemy hit
    /// </summary>
    public void EnemyHitVFX()
    {
        hitVFX.Play();
    }
    #endregion

    #region StunVFX
    bool stunEnable;

    /// <summary>
    /// Funzione che fa partire la coroutine EnemyStunVFXCoroutine
    /// </summary>
    public void EnemyStunVFX(bool _enable)
    {
        stunEnable = _enable;
        StartCoroutine(EnemyStunVFXCoroutine());
    }

    /// <summary>
    /// Coroutine che fa partire il VFX di stun
    /// </summary>
    /// <returns></returns>
    private IEnumerator EnemyStunVFXCoroutine()
    {
        stunVfx.Play();
        while (stunEnable)
        {
            yield return null;
        }
        stunVfx.Stop();
    }
    #endregion

    #region DeathVFX
    /// <summary>
    /// Funzione che esegue la coroutine EnemyDeathVFXCoroutine 
    /// </summary>
    public void EnemyDeathVFX()
    {
        StartCoroutine(EnemyDeathVFXCoroutine());
    }

    /// <summary>
    /// Funzione che fa partire il VFX di morte
    /// </summary>
    /// <returns></returns>
    private IEnumerator EnemyDeathVFXCoroutine()
    {
        deathVFX.Play();
        yield return new WaitForSeconds(deathVFX.main.duration);
        deathVFX.Stop();
    }
    #endregion

    private void OnDisable()
    {
        enemy.OnEnemyHit -= EnemyHitVFX;
    }
}
