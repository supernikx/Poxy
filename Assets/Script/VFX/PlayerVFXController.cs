using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVFXController : MonoBehaviour
{
    #region Delegates
    public static Action OnDeathVFXEnd;
    #endregion

    [Header("Death VFX")]
    [SerializeField]
    private ParticleSystem deathGhost;
    [SerializeField]
    private ParticleSystem deathSmoke;

    private Player player;

    public void Init(Player _player)
    {        
        player = _player;
        player.OnPlayerDeath += PlayDeathVFX;
        StopAllVFX();
    }

    /// <summary>
    /// Funzione che stoppa tutti i vfx
    /// </summary>
    public void StopAllVFX()
    {
        //Death VFX
        deathSmoke.Stop();
        deathGhost.Stop();
    }

    #region DeathVFX
    /// <summary>
    /// Funzione che fa partire la coroutine DeathVFXCoroutine
    /// </summary>
    public void PlayDeathVFX()
    {
        Debug.Log("test");
        StartCoroutine(DeathVFXCoroutine());
    }

    /// <summary>
    /// Coroutine che fa partire i VFX di morte e esegue l'evento OnDeathVFXEnd al termine
    /// </summary>
    /// <returns></returns>
    private IEnumerator DeathVFXCoroutine()
    {
        deathSmoke.Play();
        yield return new WaitForSeconds(0.1f);
        deathGhost.Play();
        yield return new WaitForSeconds(deathGhost.main.duration);
        deathSmoke.Stop();
        deathGhost.Stop();
        if (OnDeathVFXEnd != null)
            OnDeathVFXEnd();
    }
    #endregion
}
