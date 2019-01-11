using UnityEngine;
using System.Collections;
using StateMachine.EnemySM;

[RequireComponent(typeof(EnemyToleranceController))]
[RequireComponent(typeof(EnemyMovementController))]
[RequireComponent(typeof(EnemyCollisionController))]
[RequireComponent(typeof(EnemyViewController))]
public abstract class EnemyBase : MonoBehaviour, IEnemy
{

    [Header("General Movement Settings")]
    [SerializeField]
    protected float roamingMovementSpeed;
    [SerializeField]
    protected float alertMovementSpeed;
    protected float movementSpeed;
    [SerializeField]
    protected float turnSpeed;
    [SerializeField]
    protected GameObject Waypoints;

    protected Vector3[] path;
    protected float WaypointOffset = 0.5f;

    [Header("Damage Settings")]
    [SerializeField]
    protected int enemyStartLife;
    protected int enemyLife;
    [SerializeField]
    protected Transform shotPosition;
    [SerializeField]
    protected ShotSettings enemyShotSettings;

    [Header("Stun Settings")]
    [SerializeField]
    protected int stunDuration;
    [SerializeField]
    protected int stunHit;
    protected int stunHitGot;

    [Header("Death Settings")]
    [SerializeField]
    protected int deathDuration;

    [Header("Other Settings")]
    [SerializeField]
    protected GameObject graphics;
    protected Vector3 startPosition;

    protected EnemyManager enemyMng;
    protected EnemySMController enemySM;

    protected EnemyToleranceController toleranceCtrl;
    protected EnemyMovementController movementCtrl;
    protected EnemyCollisionController collisionCtrl;
    protected EnemyViewController viewCtrl;

    #region API
    /// <summary>
    /// Initialize Script
    /// </summary>
    public virtual void Init(EnemyManager _enemyMng)
    {
        enemyMng = _enemyMng;
        startPosition = transform.position;

        ResetData();

        // Initialize Enemy State Machine
        enemySM = GetComponent<EnemySMController>();
        if (enemySM != null)
            enemySM.Init(this, enemyMng);

        toleranceCtrl = GetComponent<EnemyToleranceController>();
        if (toleranceCtrl != null)
            toleranceCtrl.Init();

        collisionCtrl = GetComponent<EnemyCollisionController>();
        if (collisionCtrl != null)
            collisionCtrl.Init();

        movementCtrl = GetComponent<EnemyMovementController>();
        if (movementCtrl != null)
            movementCtrl.Init(collisionCtrl);

        viewCtrl = GetComponent<EnemyViewController>();
        if (viewCtrl != null)
            viewCtrl.Init();

        SetPath();

    }

    /// <summary>
    /// Funzione che si ovvupa del movimento in stato di roaming
    /// </summary>
    public abstract void MoveRoaming(bool _enabled);

    /// <summary>
    /// Funzione che si ovvupa del movimento in stato di alert
    /// Se restituisce false, il player non è più in vista
    /// </summary>
    public abstract void AlertActions(bool _enabled);

    /// <summary>
    /// Funzione che invia il nemico in stato di allerta
    /// </summary>
    public void Alert()
    {
        if (enemySM.GoToAlert != null)
            enemySM.GoToAlert();
    }

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
        if (enemySM.GoToDeath != null)
            enemySM.GoToDeath();
    }
    #endregion

    #region Stun
    /// <summary>
    /// Funzione che aumenta di 1 i colpi stun ricevuti dal nemico
    /// </summary>
    public void StunHit()
    {
        stunHitGot++;
        if (stunHitGot == stunHit && EnemyManager.OnEnemyStun != null)
        {
            EnemyManager.OnEnemyStun(this);
            stunHitGot = 0;
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
    #endregion

    #region Damage
    /// <summary>
    /// Funzione che toglie al nemico i danni del proiettile
    /// </summary>
    public void DamageHit(IBullet _bullet)
    {
        enemyLife = Mathf.Clamp(enemyLife - _bullet.GetBulletDamage(), 0, enemyStartLife);
        if (enemyLife == 0 && EnemyManager.OnEnemyDeath != null)
        {
            EnemyManager.OnEnemyDeath(this);
        }
    }

    /// <summary>
    /// Funzione che manda il nemico in stato Morte
    /// </summary>
    public void Die()
    {
        stunHitGot = 0;
        if (enemySM.GoToDeath != null)
            enemySM.GoToDeath();
    }
    #endregion

    #region Reset
    /// <summary>
    /// Funzione che reimposta i dati con i valori di default
    /// </summary>
    public void ResetData()
    {
        stunHitGot = 0;
        enemyLife = enemyStartLife;
    }

    /// <summary>
    /// Funzione che reimposta la posizione del nemico con quella iniziale
    /// </summary>
    public void ResetPosition()
    {
        transform.position = startPosition;
    }
    #endregion

    #region Getters
    /// <summary>
    /// Funzione che ritorna il tipo di sparo del nemico
    /// </summary>
    /// <returns></returns>
    public ObjectTypes GetShotType()
    {
        return enemyShotSettings.bulletType;
    }

    /// <summary>
    /// Get stun duration
    /// </summary>
    public int GetStunDuration()
    {
        return stunDuration;
    }

    /// <summary>
    /// Get Death Duration
    /// </summary>
    public int GetDeathDuration()
    {
        return deathDuration;
    }

    /// <summary>
    /// Funzione che ritorna il danno del nemico
    /// </summary>
    /// <returns></returns>
    public int GetDamage()
    {
        return enemyShotSettings.damage;
    }

    /// <summary>
    /// Get Graphics Reference
    /// </summary>
    public GameObject GetGraphics()
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
        return GetComponent<Collider>();
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
    #endregion
    #endregion

    #region Waypoints
    /// <summary>
    /// Funzione che imposta il Path del nemico
    /// </summary>
    protected void SetPath()
    {
        int _childCount = Waypoints.transform.childCount;
        path = new Vector3[_childCount];
        for (int i = 0; i < _childCount; i++)
        {
            path[i] = Waypoints.transform.GetChild(i).position;
        }
        currentWaypoint = 0;
    }

    /// <summary>
    /// Funzione che ritorna la posizione del waypoint successivo
    /// </summary>
    int currentWaypoint;
    protected Vector3 GetNextWaypointPosition()
    {
        if (currentWaypoint + 1 >= path.Length)
            currentWaypoint = 0;
        else
            currentWaypoint++;
        return path[currentWaypoint];
    }
    #endregion
}
