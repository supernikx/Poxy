using UnityEngine;
using System;
using System.Collections;

public class TurretAnimationController : MonoBehaviour
{
    private Animator anim;
    private Turret turret;

    public void ResetAnimator()
    {
        GoToIdle();
    }

    #region Animations
    private void GoToShoot()
    {
        anim.SetTrigger("GoToShoot");
    }

    private void GoToIdle()
    {
        anim.SetTrigger("GoToIdle");
    }
    
    private void GoToDeath()
    {
        anim.SetTrigger("GoToDeath");
    }
    #endregion

    #region API
    public void Init(Turret _turret)
    {
        turret = _turret;
        anim = GetComponentInChildren<Animator>();

        turret.OnEnemyShot += ShotAnimation;
        turret.OnEnemyDeath += DeathAnimation;
    }
    #endregion

    #region Shot
    Action shotAnimationCallback;
    public void ShotAnimation(Action _OnShotAnimationEndCallback )
    {
        shotAnimationCallback = _OnShotAnimationEndCallback;
        GoToShoot();
    }

    public void HandleShotAnimationEnd()
    {
        if (shotAnimationCallback != null)
            shotAnimationCallback();

        shotAnimationCallback = null;
    }
    #endregion

    #region DeathAnimation
    Action deathAnimationCallback;
    public void DeathAnimation(Action _OnDeathAnimationEndCallback)
    {
        deathAnimationCallback = _OnDeathAnimationEndCallback;
        GoToDeath();
    }

    public void HandleDeathAnimationEnd()
    {
        if (deathAnimationCallback != null)
            deathAnimationCallback();

        deathAnimationCallback = null;
    }
    #endregion
}
