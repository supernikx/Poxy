using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Animator))]
public class Walker : EnemyBase
{
    bool CanShot;

    private IEnumerator FiringRateCoroutine()
    {
        CanShot = false;
        yield return new WaitForSeconds(1 / enemyShotSettings.firingRate);
        CanShot = true;
    }

    #region API
    public override void Init(EnemyManager _enemyMng)
    {
        base.Init(_enemyMng);
        CanShot = true;
    }

    /// <summary>
    /// Funzione che controlla se puoi sparare e ritorna true o false
    /// </summary>
    /// <param name="_target"></param>
    /// <returns></returns>
    public override bool CheckShot(Transform _target)
    {
        IBullet bullet = PoolManager.instance.GetPooledObject(enemyShotSettings.bulletType, gameObject).GetComponent<IBullet>();
        if ((bullet as ParabolicBullet).CheckShotRange(_target.position, shotPosition.position, enemyShotSettings.shotSpeed))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Funzione che fa sparare il nemico e ritorna true se spara, altrimenti false
    /// </summary>
    /// <param name="_target"></param>
    /// <returns></returns>
    public override bool Shot(Transform _target)
    {
        if (CanShot)
        {
            IBullet bullet = PoolManager.instance.GetPooledObject(enemyShotSettings.bulletType, gameObject).GetComponent<IBullet>();
            animCtrl.ShotAnimation();
            bullet.Shot(enemyShotSettings.damage, enemyShotSettings.shotSpeed, 5f, shotPosition.position, _target);
            StartCoroutine(FiringRateCoroutine());
            return true;
        }
        return false;
    }
    #endregion
}
