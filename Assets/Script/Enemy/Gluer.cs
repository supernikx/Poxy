using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Animator))]
public class Gluer : EnemyBase
{
    bool CanShot;

    [Header("Enemy Specifics Settings")]
    [SerializeField]
    private float roamingFiringRate;
    [SerializeField]
    private float shotRadius;
    private IEnumerator roamingFiringRateCoroutine;


    private IEnumerator FiringRateCoroutine(float _firingRate)
    {
        CanShot = false;
        yield return new WaitForSeconds(1 / _firingRate);
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
        float _distance = Vector3.Distance(_target.position, shotPosition.position);
        if (_distance <= shotRadius)
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
            Vector3 _direction = (_target.position - shotPosition.position);
            bullet.Shot(enemyShotSettings.damage, enemyShotSettings.shotSpeed, shotRadius, shotPosition.position, _direction);
            animCtrl.ShotAnimation();
            StartCoroutine(FiringRateCoroutine(enemyShotSettings.firingRate));
            return true;
        }
        return false;
    }
    #endregion
}
