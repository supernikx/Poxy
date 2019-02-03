using UnityEngine;
using System.Collections;
using StateMachine.EnemySM;
using DG.Tweening;

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
    protected GameObject wayPoint;
    private float pathLenght;

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
    public int stunHitGot;

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

        ResetLife();
        ResetStunHit();

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

        CalculatePathLenght();

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
        if (stunHitGot < stunHit)
        {
            stunHitGot++;
            if (stunHitGot == stunHit && EnemyManager.OnEnemyStun != null)
            {
                EnemyManager.OnEnemyStun(this);
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
    #endregion

    #region Damage
    /// <summary>
    /// Funzione che toglie al nemico i danni del proiettile
    /// </summary>
    public void DamageHit(int _damage, float _time = 0)
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
    private IEnumerator LoseHealthOverTime(int _damage, float _time)
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
    }

    /// <summary>
    /// Funzione che reimposta gli stunhit
    /// </summary> 
    public void ResetStunHit()
    {
        stunHitGot = 0;
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
    /// Funzione che ritorna la vita del nemico
    /// </summary>
    /// <returns></returns>
    public int GetHealth()
    {
        return enemyLife;
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

    #region Movement Roaming
    /// <summary>
    /// Funzione che calcola la lunghezza del path del nemico
    /// </summary>
    protected void CalculatePathLenght()
    {
        pathLenght = Mathf.Abs(wayPoint.transform.position.x - transform.position.x);
    }

    /// <summary>
    /// Funzione che ritorna la lunghezza del path
    /// </summary>
    /// <returns></returns>
    protected float GetPathLenght()
    {
        return pathLenght;
    }

    IEnumerator moveRoaming;
    /// <summary>
    /// Funzione che si ovvupa si attivare/disattivare il movimento in stato di roaming
    /// </summary>
    public void MoveRoaming(bool _enabled)
    {
        if (_enabled)
        {
            movementSpeed = roamingMovementSpeed;
            moveRoaming = MoveRoamingCoroutine();
            StartCoroutine(moveRoaming);
        }
        else
        {
            StopCoroutine(moveRoaming);
            transform.DOKill();
        }
    }

    /// <summary>
    /// Coroutine che gestisce il movimento del nemico
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator MoveRoamingCoroutine()
    {
        Vector3 movementVector = Vector3.zero;
        float pathLenght = GetPathLenght();
        float pathTraveled = 0f;
        bool movementBlocked = false;

        while (true)
        {
            if (pathTraveled >= pathLenght - 0.1f)
            {
                pathTraveled = 0f;
                movementBlocked = false;
                Vector3 rotationVector = Vector3.zero;
                if (transform.rotation.y == 0)
                    rotationVector.y = 180f;
                yield return transform.DORotateQuaternion(Quaternion.Euler(rotationVector), turnSpeed)
                    .SetEase(Ease.Linear)
                    .OnUpdate(() => movementCtrl.MovementCheck())
                    .OnPause(() => StartCoroutine(MoveRoamingStickyChecker(true)))
                    .OnPlay(() => StartCoroutine(MoveRoamingStickyChecker(false)))
                    .WaitForCompletion();
            }

            //Movimento Nemico                
            movementVector.x = movementSpeed;
            Vector3 distanceTraveled = MoveRoamingUpdate(movementVector);

            if ((distanceTraveled - Vector3.zero).sqrMagnitude < 0.001f && movementBlocked == false)
                movementBlocked = true;
            else if ((distanceTraveled - Vector3.zero).sqrMagnitude < 0.001f && movementBlocked == true)
            {
                pathTraveled = pathLenght;
            }
            else if ((distanceTraveled - Vector3.zero).sqrMagnitude > 0.001f && movementBlocked == true)
                movementBlocked = false;

            pathTraveled += distanceTraveled.x;
            yield return new WaitForFixedUpdate();
        }
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

    protected abstract Vector3 MoveRoamingUpdate(Vector3? movementVector = null);
    #endregion

    #region Movement Alert
    /// <summary>
    /// Funzione che invia il nemico in stato di allerta
    /// </summary>
    public void Alert()
    {
        if (enemySM.GoToAlert != null)
            enemySM.GoToAlert();
    }

    IEnumerator alertCoroutine;
    /// <summary>
    /// Funzione che si ovvupa del movimento in stato di alert
    /// Se restituisce false, il player non è più in vista
    /// </summary>
    public virtual void AlertActions(bool _enable)
    {
        if (_enable)
        {
            movementSpeed = alertMovementSpeed;
            alertCoroutine = AlertActionCoroutine();
            StartCoroutine(alertCoroutine);
        }
        else
        {
            StopCoroutine(alertCoroutine);
            transform.DOKill();
        }

    }
    protected abstract IEnumerator AlertActionCoroutine();
    #endregion
    #endregion
}
