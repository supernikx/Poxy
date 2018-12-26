using UnityEngine;
using System.Collections;
using StateMachine.EnemySM;

[RequireComponent(typeof(EnemyToleranceController))]
[RequireComponent(typeof(EnemyMovementController))]
[RequireComponent(typeof(EnemyCollisionController))]
public abstract class EnemyBase : MonoBehaviour, IEnemy
{

    [Header("General Movement Settings")]
    protected float movementSpeed;
    [SerializeField]
    protected GameObject Waypoints;
    [SerializeField]
    protected float TimeToCompletePath;
    [SerializeField]
    protected float WaypointOffset;

    protected float[] path;
    protected int nextWaypoint;
    protected float horizontalVelocity = 0;

    [Header("Stun Settings")]
    [SerializeField]
    protected int stunDuration;

    [Header("Death Settings")]
    [SerializeField]
    protected int deathDuration;

    [Header("Other Settings")]
    [SerializeField]
    protected GameObject graphics;

    protected EnemyManager enemyMng;
    protected EnemySMController enemySM;
    
    protected EnemyToleranceController toleranceCtrl;
    protected EnemyMovementController movementCtrl;
    protected EnemyCollisionController collisionCtrl;


    #region API
    /// <summary>
    /// Initialize Script
    /// </summary>
    public virtual void Init(EnemyManager _enemyMng)
    {
        enemyMng = _enemyMng;

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

        SetPath();

    }

    public void SetPath()
    {
        int _childCount = Waypoints.transform.childCount;
        path = new float[_childCount];
        for (int i = 0; i < _childCount; i++)
        {
            path[i] = Waypoints.transform.GetChild(i).position.x;
        }

        nextWaypoint = 0;
        float _distance = 0;

        for (int i = 0; i < path.Length; i++)
        {
            int j = i + 1;

            if (path.Length == j)
            {
                j = 0;
            }

            _distance += Mathf.Abs(path[i] - path[j]);
        }

        horizontalVelocity = _distance / TimeToCompletePath;
    }

    /// <summary>
    /// Funzione che si ovvupa del movimento
    /// </summary>
    public abstract void Move();

    /// <summary>
    /// Funzione che si ovvupa di bloccare il movimento
    /// </summary>
    public abstract void Stop();

    /// <summary>
    /// Funzione che manda il nemico in stato Stun
    /// </summary>
    public void Stun()
    {
        if (enemySM.GoToStun != null)
            enemySM.GoToStun();
    }

    /// <summary>
    /// Funzione che manda il nemico in stato Morte
    /// </summary>
    public void Die()
    {
        if (enemySM.GoToDeath != null)
            enemySM.GoToDeath();
    }

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
    /// 
    /// </summary>
    public void StartTolerance()
    {
        toleranceCtrl.SetCanStart(true);
    }

    #region Getters
    /// <summary>
    /// Get stun duration
    /// </summary>
    public virtual int GetStunDuration()
    {
        return stunDuration;
    }

    /// <summary>
    /// Get Death Duration
    /// </summary>
    public virtual int GetDeathDuration()
    {
        return deathDuration;
    }

    /// <summary>
    /// Get Graphics Reference
    /// </summary>
    public virtual GameObject GetGraphics()
    {
        return graphics;
    }

    /// <summary>
    /// Get Collider Reference
    /// </summary>
    public virtual BoxCollider GetCollider()
    {
        return GetComponent<BoxCollider>();
    }

    public virtual EnemyToleranceController GetToleranceCtrl()
    {
        return toleranceCtrl;
    }

    public virtual EnemyMovementController GetMovementCtrl()
    {
        return movementCtrl;
    }

    public virtual EnemyCollisionController GetCollisionCtrl()
    {
        return collisionCtrl;
    }
    #endregion
    #endregion
}
