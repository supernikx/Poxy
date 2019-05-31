using UnityEngine;
using System.Collections;
using System;
using StateMachine.EnemySM;
using DG.Tweening;

[RequireComponent(typeof(EnemyToleranceController))]
[RequireComponent(typeof(EnemyMovementController))]
[RequireComponent(typeof(EnemyCollisionController))]
[RequireComponent(typeof(EnemyViewController))]
public abstract class EnemyBase : MonoBehaviour, IEnemy, IControllable
{
    #region Delegates
    public Action OnEnemyHit;
    public Action<Action> OnEnemyShot;
    #endregion

    [Header("General Movement Settings")]
    [SerializeField]
    protected float roamingMovementSpeed;
    [SerializeField]
    protected float alertMovementSpeed;
    protected float movementSpeed;
    [SerializeField]
    protected GameObject wayPoint;
    private float pathLenght;

    [Header("Damage Settings")]
    [SerializeField]
    protected float enemyStartLife;
    protected float enemyLife;
    [SerializeField]
    protected Transform shotPosition;
    [SerializeField]
    protected ShotSettings enemyShotSettings;

    [Header("Stun Settings")]
    [SerializeField]
    protected float stunDuration;
    [SerializeField]
    protected int stunHit;
    protected int stunHitGot;

    [Header("Death Settings")]
    [SerializeField]
    protected float defaultRespawnTime;
    protected float respawnTime;
    [SerializeField]
    protected float speedrunBonusTime;

    protected EnemyGraphicController graphics;
    protected Vector3 startPosition;
    protected Quaternion startRotation;

    protected EnemyManager enemyMng;
    protected EnemySMController enemySM;
    protected EnemyAnimationController animCtrl;
    protected EnemyVFXController vfxCtrl;
    protected EnemyCommandsSpriteController commandsSpriteCtrl;
    protected EnemyToleranceController toleranceCtrl;
    protected EnemyMovementController movementCtrl;
    protected EnemyCollisionController collisionCtrl;
    protected EnemyViewController viewCtrl;

    protected ControllableType controllableType = ControllableType.Enemy;

    #region API
    /// <summary>
    /// Initialize Script
    /// </summary>
    public virtual void Init(EnemyManager _enemyMng)
    {
        graphics = GetComponentInChildren<EnemyGraphicController>();
        enemyMng = _enemyMng;
        startPosition = transform.position;
        startRotation = transform.rotation;

        ResetLife();
        ResetStunHit();
        ResetPosition();
        SetCanStun(true);

        // Initialize Enemy State Machine
        enemySM = GetComponent<EnemySMController>();
        if (enemySM != null)
            enemySM.Init(this);

        toleranceCtrl = GetComponent<EnemyToleranceController>();
        if (toleranceCtrl != null)
            toleranceCtrl.Init();

        collisionCtrl = GetComponent<EnemyCollisionController>();
        if (collisionCtrl != null)
            collisionCtrl.Init();

        movementCtrl = GetComponent<EnemyMovementController>();
        if (movementCtrl != null)
            movementCtrl.Init(collisionCtrl);

        animCtrl = GetComponentInChildren<EnemyAnimationController>();
        if (animCtrl != null)
            animCtrl.Init(this);

        commandsSpriteCtrl = GetComponentInChildren<EnemyCommandsSpriteController>();
        if (commandsSpriteCtrl != null)
            commandsSpriteCtrl.Init();

        vfxCtrl = GetComponentInChildren<EnemyVFXController>();
        if (vfxCtrl != null)
            vfxCtrl.Init(this);

        viewCtrl = GetComponent<EnemyViewController>();
        if (viewCtrl != null)
            viewCtrl.Init();

        CalculatePathLenght();
    }

    /// <summary>
    /// Funzione che segue il knockback
    /// </summary>
    /// <param name="_dir"></param>
    /// <param name="_force"></param>
    public void ApplyKnockback(Vector3 _dir, float _force)
    {
        if (collisionCtrl.GetCollisionInfo().StickyCollision())
            return;

        Vector3 knockbackVector = Vector3.zero;
        if (Mathf.Sign(_dir.x) == Mathf.Sign(transform.right.normalized.x))
            knockbackVector.x = _force;
        else
            knockbackVector.x = -_force;

        movementCtrl.MovementCheck(knockbackVector);
    }

    #region StateHandler
    /// <summary>
    /// Funzione che avvisa l'ingresso in roaming state
    /// </summary>
    public void EnemyRoamingState()
    {
        movementSpeed = roamingMovementSpeed;
    }

    /// <summary>
    /// Funzione che avvisa l'ingresso in alert state
    /// </summary>
    public void EnemyAlertState()
    {
        movementSpeed = alertMovementSpeed;
    }
    #endregion

    #region Parasite
    /// <summary>
    /// Funzione che manda il nemico in stato parassita
    /// </summary>
    /// <param name="_player"></param>
    public void Parasite(Player _player)
    {
        if (enemySM.GoToParasite != null)
            enemySM.GoToParasite(_player);
    }

    /// <summary>
    /// Funzione che manda il nemico in stato di Morte
    /// </summary>
    public void EndParasite()
    {
        if (EnemyManager.OnEnemyDeath != null)
            EnemyManager.OnEnemyDeath(this);
    }
    #endregion

    #region Stun
    bool canStun;
    /// <summary>
    /// Funzione che aumenta di 1 i colpi stun ricevuti dal nemico
    /// </summary>
    public void StunHit()
    {
        if (canStun)
        {
            if (stunHitGot < stunHit)
            {
                stunHitGot++;
                if (stunHitGot == stunHit && EnemyManager.OnEnemyStun != null)
                {
                    EnemyManager.OnEnemyStun(this);
                }
            }
        }
    }

    /// <summary>
    /// Funzione che manda il nemico in stato Stun
    /// </summary>
    public void Stun()
    {
        if (enemySM.GoToStun != null)
            enemySM.GoToStun();
    }

    public void SetCanStun(bool _switch)
    {
        canStun = _switch;
    }
    #endregion

    #region Damage
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

    /// <summary>
    /// Funzione che manda il nemico in stato Morte
    /// </summary>
    public void Die(float _respawnTime = -1)
    {
        if (_respawnTime == -1)
            respawnTime = defaultRespawnTime;
        else
            respawnTime = _respawnTime;

        stunHitGot = 0;

        if (SpeedrunManager.StopForSeconds != null)
            SpeedrunManager.StopForSeconds(speedrunBonusTime);

        if (enemySM.GoToDeath != null)
        {
            enemySM.GoToDeath();
        }
    }
    #endregion

    #region Reset
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
        stunHitGot = 0;
    }

    /// <summary>
    /// Funzione che riporta il nemico in stato di roaming
    /// </summary>
    public void Respawn()
    {
        if (enemySM.GoToRoaming != null)
            enemySM.GoToRoaming();
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
    /// Funzione che ritorna lo shot point del nemico
    /// </summary>
    /// <returns></returns>
    public Transform GetShotPoint()
    {
        return shotPosition;
    }

    /// <summary>
    /// Get stun duration
    /// </summary>
    public float GetStunDuration()
    {
        return stunDuration;
    }

    /// <summary>
    /// Get Death Duration
    /// </summary>
    public float GetRespawnDuration()
    {
        return respawnTime;
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
    /// Funzione che ritorna la movement speed del nemcio
    /// </summary>
    /// <returns></returns>
    public float GetMovementSpeed()
    {
        return movementSpeed;
    }

    /// <summary>
    /// Funzione che ritorna la lunghezza del path
    /// </summary>
    /// <returns></returns>
    public float GetPathLenght()
    {
        return pathLenght;
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
    /// Get Graphics Reference
    /// </summary>
    public IGraphic GetGraphics()
    {
        return graphics;
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
    /// Funzione che ritorna il layer dei nemici
    /// </summary>
    /// <returns></returns>
    public int GetEnemyDefaultLayer()
    {
        return enemyMng.GetEnemyLayer();
    }

    /// <summary>
    /// Get Collider Reference
    /// </summary>
    public Collider GetCollider()
    {
        return collisionCtrl.GetCollider();
    }

    /// <summary>
    /// Funzione che ritorna il Tollerance Bar Controller
    /// </summary>
    /// <returns></returns>
    public EnemyToleranceController GetToleranceCtrl()
    {
        return toleranceCtrl;
    }

    /// <summary>
    /// Funzione che ritorna il Movement Controller
    /// </summary>
    /// <returns></returns>
    public EnemyMovementController GetMovementCtrl()
    {
        return movementCtrl;
    }

    /// <summary>
    /// Funzione che ritorna l'animation controller
    /// </summary>
    /// <returns></returns>
    public EnemyAnimationController GetAnimationController()
    {
        return animCtrl;
    }

    /// <summary>
    /// Funzione che ritorna il command sprite controller
    /// </summary>
    /// <returns></returns>
    public EnemyCommandsSpriteController GetEnemyCommandsSpriteController()
    {
        return commandsSpriteCtrl;
    }

    /// <summary>
    /// Funzione che ritorna il vfx controller del nemico
    /// </summary>
    /// <returns></returns>
    public EnemyVFXController GetVFXController()
    {
        return vfxCtrl;
    }

    /// <summary>
    /// Funzione che ritorna il Collision Controller
    /// </summary>
    /// <returns></returns>
    public EnemyCollisionController GetCollisionCtrl()
    {
        return collisionCtrl;
    }

    /// <summary>
    /// Funzione che ritorna il View Controller
    /// </summary>
    /// <returns></returns>
    public EnemyViewController GetViewCtrl()
    {
        return viewCtrl;
    }

    /// <summary>
    /// Funzione che ritorna il controllable type
    /// </summary>
    /// <returns></returns>
    public ControllableType GetControllableType()
    {
        return controllableType;
    }
    #endregion

    #region Movement Roaming
    /// <summary>
    /// Funzione che calcola la lunghezza del path del nemico
    /// </summary>
    protected void CalculatePathLenght()
    {
        pathLenght = Mathf.Abs(wayPoint.transform.position.x - transform.position.x);
    }

    /// <summary>
    /// Funzione che controlla se si è attaccati ad un oggetto sticky
    /// </summary>
    /// <param name="_pause"></param>
    /// <returns></returns>
    protected IEnumerator MoveRoamingStickyChecker(bool _pause)
    {
        if (_pause)
        {
            while (collisionCtrl.GetCollisionInfo().StickyCollision())
            {
                yield return null;
            }
            transform.DOPlay();
        }
        else
        {
            while (!collisionCtrl.GetCollisionInfo().StickyCollision())
            {
                yield return null;
            }
            transform.DOPause();
        }
    }
    #endregion

    #region Shot
    /// <summary>
    /// Funzione che fa sparare il nemico e ritorna true se spara, altrimenti false
    /// </summary>
    /// <param name="_target"></param>
    /// <returns></returns>
    public abstract bool Shot(Transform _target);

    /// <summary>
    /// Funzione che controlla se puoi sparare e ritorna true o false
    /// </summary>
    /// <param name="_target"></param>
    /// <returns></returns>
    public abstract bool CheckShot(Transform _target);

    /// <summary>
    /// Funzione che controlla se sei in range di sparo e ritorna true o false
    /// </summary>
    /// <param name="_target"></param>
    /// <returns></returns>
    public abstract bool CheckRange(Transform _target);
    #endregion
    #endregion
}
