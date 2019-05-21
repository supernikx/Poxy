using UnityEngine;
using UnityEditor;
using System.Collections;

public class Turret : MonoBehaviour, IEnemy
{
    [Header("Damage Settings")]
    [SerializeField]
    protected float enemyStartLife;
    protected float enemyLife;
    [SerializeField]
    protected Transform shotPosition;
    [SerializeField]
    protected ShotSettings enemyShotSettings;

    [Header("Death Settings")]
    [SerializeField]
    protected float defaultRespawnTime;
    protected float respawnTime;

    protected EnemyGraphicController graphics;
    protected Vector3 startPosition;
    protected Quaternion startRotation;
    protected Collider collider;
    protected EnemyManager enemyMng;
    protected EnemyViewController viewCtrl;

    //Transform targetPos;
    /// <summary>
    /// Funzione che aspetta la fine dell'animazione di sparo per sparare effettivamente il proiettile
    /// </summary>
    //private void HandleShotAnimationEnd()
    //{
    //    IBullet bullet = PoolManager.instance.GetPooledObject(enemyShotSettings.bulletType, gameObject).GetComponent<IBullet>();
    //    Vector3 shotPosToCheck = shotPosition.position;
    //    shotPosToCheck.z = targetPos.position.z;
    //    Vector3 _direction = (targetPos.position - shotPosToCheck);
    //    bullet.Shot(enemyShotSettings.damage, enemyShotSettings.shotSpeed, enemyShotSettings.range, shotPosition.position, _direction);
    //}

    #region API
    /// <summary>
    /// Initialize Script
    /// </summary>
    public void Init(EnemyManager _enemyMng)
    {
        graphics = GetComponentInChildren<EnemyGraphicController>();
        collider = GetComponent<Collider>();
        enemyMng = _enemyMng;
        startPosition = transform.position;
        startRotation = transform.rotation;
        CanShot = true;

        ResetLife();
        ResetStunHit();
        ResetPosition();

        viewCtrl = GetComponent<EnemyViewController>();
        if (viewCtrl != null)
            viewCtrl.Init();

        behaviourCoroutine = StartCoroutine(NormalBehaviour());
    }

    /// <summary>
    /// Funzione che toglie al nemico i danni del proiettile
    /// </summary>
    public void DamageHit(float _damage, float _time = 0)
    {
        if (_time == 0)
        {
            enemyLife = Mathf.Clamp(enemyLife - _damage, 0, enemyStartLife);
            if (enemyLife == 0 && EnemyManager.OnEnemyDeath != null)
            {
                EnemyManager.OnEnemyDeath(this);
            }
        }
        else
        {
            StartCoroutine(LoseHealthOverTime(_damage, _time));
        }
    }

    public void Die(float _respawnTime = -1)
    {
        StopCoroutine(behaviourCoroutine);
        behaviourCoroutine = null;

        collider.enabled = false;
        ResetLife();
        ResetStunHit();
        graphics.Disable();

        if (_respawnTime == -1)
            respawnTime = defaultRespawnTime;
        else
            respawnTime = _respawnTime;

        deathCoroutine = StartCoroutine(DeathCoroutine());
    }

    /// <summary>
    /// Funzione che reimposta i dati con i valori di default
    /// </summary>
    public void ResetLife()
    {
        enemyLife = enemyStartLife;
    }

    /// <summary>
    /// Funzione che reimposta la posizione del nemico con quella iniziale
    /// </summary>
    public void ResetPosition()
    {
        transform.position = startPosition;
        transform.rotation = startRotation;
        graphics.gameObject.transform.localEulerAngles = new Vector3(0, 0, 0);
    }

    /// <summary>
    /// Funzione che reimposta gli stunhit
    /// </summary> 
    public void ResetStunHit()
    {
        return;
    }

    public void Respawn()
    {
        if (deathCoroutine != null)
        {
            StopCoroutine(deathCoroutine);
            deathCoroutine = null; 
        }

        EnemyManager.OnEnemyEndDeath(this);

        ResetPosition();
        graphics.Enable();
        collider.enabled = true;

        behaviourCoroutine = StartCoroutine(NormalBehaviour());
    }
    
    public void StunHit()
    {
        return;
    }

    #region Shot
    private bool CanShot;

    public bool CheckShot(Transform _target)
    {
        float _distance = Vector3.Distance(_target.position, shotPosition.position);
        if (_distance <= enemyShotSettings.range)
        {
            return true;
        }
        return false;
    }

    public bool Shot(Transform _target)
    {
        if (CanShot)
        {
            //targetPos = _target;
            StartCoroutine(FiringRateCoroutine(enemyShotSettings.firingRate));
            //if (OnEnemyShot != null)
            //    OnEnemyShot(HandleShotAnimationEnd);
            IBullet bullet = PoolManager.instance.GetPooledObject(enemyShotSettings.bulletType, gameObject).GetComponent<IBullet>();
            Vector3 shotPosToCheck = shotPosition.position;
            shotPosToCheck.z = _target.position.z;
            Vector3 _direction = (_target.position - shotPosToCheck);
            bullet.Shot(enemyShotSettings.damage, enemyShotSettings.shotSpeed, enemyShotSettings.range, shotPosition.position, _direction);
            return true;
        }
        return false;
    }
    #endregion

    #region Unused
    public void Stun()
    {
        return;
    }
    #endregion
    #endregion

    #region Coroutines
    private Coroutine behaviourCoroutine;
    private IEnumerator NormalBehaviour()
    {
        while (true)
        {
            Transform _target = viewCtrl.FindPlayer();

            if (_target != null && viewCtrl.CanSeePlayer(_target.position) && CheckShot(_target))
            {
                Shot(_target);
            }

            yield return null; 
        }
    }

    private Coroutine deathCoroutine;
    private IEnumerator DeathCoroutine()
    {
        float _timer = 0;

        while (_timer < respawnTime)
        {
            _timer += Time.deltaTime;
            yield return null;
        }

        Respawn();
    }

    /// <summary>
    /// Coroutine che fa perdere vita overtime al nemico
    /// </summary>
    /// <param name="_damage"></param>
    /// <param name="_time"></param>
    /// <returns></returns>
    private IEnumerator LoseHealthOverTime(float _damage, float _time)
    {
        float tickDuration = 0.5f;
        float damgeEachTick = tickDuration * _damage / _time;
        int ticks = Mathf.RoundToInt(_time / tickDuration);
        int tickCounter = 0;
        while (tickCounter < ticks)
        {
            enemyLife = Mathf.RoundToInt(Mathf.Clamp(enemyLife - damgeEachTick, 0, enemyStartLife));
            tickCounter++;
            yield return new WaitForSeconds(tickDuration);
        }
    }
    
    private IEnumerator FiringRateCoroutine(float _firingRate)
    {
        CanShot = false;
        yield return new WaitForSeconds(1 / _firingRate);
        CanShot = true;
    }
    #endregion

    #region Getters
    /// <summary>
    /// Funzione che ritorna il tipo di sparo del nemico
    /// </summary>
    /// <returns></returns>
    public ObjectTypes GetBulletType()
    {
        return enemyShotSettings.bulletType;
    }

    /// <summary>
    /// Get Collider Reference
    /// </summary>
    public Collider GetCollider()
    {
        return collider;
    }
    
    /// <summary>
    /// Funzione che ritorna il danno del nemico
    /// </summary>
    /// <returns></returns>
    public float GetDamage()
    {
        return enemyShotSettings.damage;
    }
    
    /// <summary>
    /// Funzione che ritorna il layer dei nemici
    /// </summary>
    /// <returns></returns>
    public int GetEnemyDefaultLayer()
    {
        return enemyMng.GetEnemyLayer();
    }

    /// <summary>
    /// Funzione che ritorna il parent del nemico
    /// </summary>
    /// <returns></returns>
    public Transform GetEnemyDefaultParent()
    {
        return enemyMng.GetEnemyParent();
    }

    /// <summary>
    /// Get Graphics Reference
    /// </summary>
    public IGraphic GetGraphics()
    {
        return graphics;
    }

    /// <summary>
    /// Funzione che ritorna la vita del nemico
    /// </summary>
    /// <returns></returns>
    public float GetHealth()
    {
        return enemyLife;
    }
    
    /// <summary>
    /// Get Death Duration
    /// </summary>
    public float GetRespawnDuration()
    {
        return respawnTime;
    }

    /// <summary>
    /// Funzione che ritorna lo shot point del nemico
    /// </summary>
    /// <returns></returns>
    public Transform GetShotPoint()
    {
        return shotPosition;
    }

    /// <summary>
    /// Funzione che ritorna il View Controller
    /// </summary>
    /// <returns></returns>
    public EnemyViewController GetViewCtrl()
    {
        return viewCtrl;
    }
    
    #region TODO
    public EnemyAnimationController GetAnimationController()
    {
        return null;
    }

    public EnemyVFXController GetVFXController()
    {
        return null;
    }
    #endregion

    #region MaybeUnused
    public EnemyCollisionController GetCollisionCtrl()
    {
        return null;
    }

    public EnemyCommandsSpriteController GetEnemyCommandsSpriteController()
    {
        return null;
    }

    public EnemyMovementController GetMovementCtrl()
    {
        return null;
    }

    public float GetMovementSpeed()
    {
        return 0;
    }

    public float GetPathLenght()
    {
        return 0;
    }
    
    public EnemyToleranceController GetToleranceCtrl()
    {
        return null;
    }

    public float GetStunDuration()
    {
        return 0;
    }
    #endregion
    #endregion

    #region Setters
    public void SetCanStun(bool _switch)
    {
        return;
    }
    #endregion

    #region Unused
    public void ApplyKnockback(Vector3 _dir, float _force)
    {
        return;
    }

    public void EndParasite()
    {
        return;
    }
    
    public void EnemyAlertState()
    {
        return;
    }

    public void EnemyRoamingState()
    {
        return;
    }

    public void Parasite(Player _player)
    {
        return;
    }
    #endregion

    
}