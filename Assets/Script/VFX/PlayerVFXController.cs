using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVFXController : MonoBehaviour
{
    #region Delegates
    public static Action OnDeathVFXEnd;
    #endregion
    [Header("Landing VFX")]
    [SerializeField]
    private ParticleSystem landingVFX;
    [Header("Hit VFX")]
    [SerializeField]
    private ParticleSystem hitVFX;
    [Header("Death VFX")]
    [SerializeField]
    private ParticleSystem deathGhostVFX;
    [SerializeField]
    private ParticleSystem deathSmokeVFX;

    private Player player;

    public void Init(Player _player)
    {
        player = _player;
        player.GetCollisionController().OnPlayerLanding += PlayerLandingVFX;
        player.OnPlayerDeath += PlayDeathVFX;
        player.OnPlayerHit += PlayerHitVFX;
        StopAllVFX();
    }

    /// <summary>
    /// Funzione che stoppa tutti i vfx
    /// </summary>
    public void StopAllVFX()
    {
        //Death VFX
        deathSmokeVFX.Stop();
        deathGhostVFX.Stop();

        //Landing VFX
        landingVFX.Stop();

        //Hit VFX
        hitVFX.Stop();
    }

    #region LandingVFX
    /// <summary>
    /// Funzione che fa paritre il VFX player landing
    /// </summary>
    private void PlayerLandingVFX()
    {
        landingVFX.Play();
    }
    #endregion

    #region HitVFX
    /// <summary>
    /// Funzione che fa paritre il VFX hitVfx
    /// </summary>
    private void PlayerHitVFX()
    {
        hitVFX.Play();
    }
    #endregion

    #region DeathVFX
    /// <summary>
    /// Funzione che fa partire la coroutine DeathVFXCoroutine
    /// </summary>
    public void PlayDeathVFX()
    {
        StartCoroutine(DeathVFXCoroutine());
    }

    /// <summary>
    /// Coroutine che fa partire i VFX di morte e esegue l'evento OnDeathVFXEnd al termine
    /// </summary>
    /// <returns></returns>
    private IEnumerator DeathVFXCoroutine()
    {
        deathSmokeVFX.Play();
        deathGhostVFX.Play();
        yield return new WaitForSeconds(deathGhostVFX.main.duration);
        deathGhostVFX.Stop();
        deathSmokeVFX.Stop();

        if (OnDeathVFXEnd != null)
            OnDeathVFXEnd();
    }
    #endregion

    private void OnDisable()
    {
        player.OnPlayerDeath -= PlayDeathVFX;
    }
}
