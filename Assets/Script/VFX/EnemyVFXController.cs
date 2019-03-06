using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVFXController : MonoBehaviour
{
    #region Delegates
    //public static Action OnDeathVFXEnd;
    #endregion

    [Header("Stun VFX")]
    [SerializeField]
    private ParticleSystem stunVfx;

    private EnemyBase enemy;

    public void Init(EnemyBase _enemy)
    {
        enemy = _enemy;
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

    #region StunVFX
    bool stunEnable;

    /// <summary>
    /// Funzione che fa partire la coroutine DeathVFXCoroutine
    /// </summary>
    public void EnemyStunVFX(bool _enable)
    {
        stunEnable = _enable;
        StartCoroutine(StunVFXCoroutine());
    }

    /// <summary>
    /// Coroutine che fa partire i VFX di morte e esegue l'evento OnDeathVFXEnd al termine
    /// </summary>
    /// <returns></returns>
    private IEnumerator StunVFXCoroutine()
    {
        stunVfx.Play();
        while (stunEnable)
        {
            yield return null;
        }
        stunVfx.Stop();
    }
    #endregion
}
