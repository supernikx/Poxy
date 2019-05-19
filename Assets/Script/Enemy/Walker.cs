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
            targetPos = _target;
            StartCoroutine(FiringRateCoroutine());
            if (OnEnemyShot != null)
                OnEnemyShot(HandleShotAnimationEnd);
            return true;
        }
        return false;
    }

    Transform targetPos;
    /// <summary>
    /// Funzione che aspetta la fine dell'animazione di sparo per sparare effettivamente il proiettile
    /// </summary>
    private void HandleShotAnimationEnd()
    {
        if (CanShot)
        {
            IBullet bullet = PoolManager.instance.GetPooledObject(enemyShotSettings.bulletType, gameObject).GetComponent<IBullet>();
            bullet.Shot(enemyShotSettings.damage, enemyShotSettings.shotSpeed, enemyShotSettings.range, shotPosition.position, targetPos);
        }
    }
    #endregion
}
