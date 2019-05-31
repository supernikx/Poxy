using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Animator))]
public class Gluer : EnemyBase
{
    private bool canShot;
    private IEnumerator roamingFiringRateCoroutine;

    private IEnumerator FiringRateCoroutine(float _firingRate)
    {
        canShot = false;
        yield return new WaitForSeconds(1 / _firingRate);
        canShot = true;
    }

    #region API
    public override void Init(EnemyManager _enemyMng)
    {
        base.Init(_enemyMng);
        canShot = true;
    }

    /// <summary>
    /// Funzione che controlla se sei in range di sparo e ritorna true o false
    /// </summary>
    /// <param name="_target"></param>
    /// <returns></returns>
    public override bool CheckRange(Transform _target)
    {
        float _distance = Vector3.Distance(_target.position, shotPosition.position);
        if (_distance <= enemyShotSettings.range)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Funzione che controlla se puoi sparare e ritorna true o false
    /// </summary>
    /// <param name="_target"></param>
    /// <returns></returns>
    public override bool CheckShot(Transform _target)
    {
        if (!canShot)
            return false;

        return CheckRange(_target);
    }

    /// <summary>
    /// Funzione che fa sparare il nemico e ritorna true se spara, altrimenti false
    /// </summary>
    /// <param name="_target"></param>
    /// <returns></returns>
    public override bool Shot(Transform _target)
    {
        if (canShot)
        {
            targetPos = _target;
            StartCoroutine(FiringRateCoroutine(enemyShotSettings.firingRate));
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
        IBullet bullet = PoolManager.instance.GetPooledObject(enemyShotSettings.bulletType, gameObject).GetComponent<IBullet>();
        Vector3 shotPosToCheck = shotPosition.position;
        shotPosToCheck.z = targetPos.position.z;
        Vector3 _direction = (targetPos.position - shotPosToCheck);
        bullet.Shot(enemyShotSettings.damage, enemyShotSettings.shotSpeed, enemyShotSettings.range, shotPosition.position, _direction);
    }
    #endregion
}
