using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BulletBase : MonoBehaviour, IPoolObject, IBullet
{
    #region IPoolObject
    public GameObject ownerObject
    {
        get
        {
            return _ownerObject;
        }
        set
        {
            _ownerObject = value;
        }
    }
    GameObject _ownerObject;

    public State CurrentState
    {
        get
        {
            return _currentState;
        }
        set
        {
            _currentState = value;
        }
    }
    State _currentState;

    public event PoolManagerEvets.Events OnObjectSpawn;
    public event PoolManagerEvets.Events OnObjectDestroy;
    #endregion
    /// <summary>
    /// Danno del proiettile
    /// </summary>
    protected int damage;
    /// <summary>
    /// Distanza massima che può percorrere il proiettile
    /// </summary>
    protected float range;
    /// <summary>
    /// Velocità a cui va il proiettile
    /// </summary>
    protected float speed;
    /// <summary>
    /// Direzione in cui va il proiettile
    /// </summary>
    protected Vector3? shotDirection;
    /// <summary>
    /// Posizione del target
    /// </summary>
    protected Vector3? targetPosition;
    /// <summary>
    /// Angolo del fucile
    /// </summary>
    protected float shotAngle;
    /// <summary>
    /// Posizione da cui parte lo sparo
    /// </summary>
    protected Vector3 shotPosition;
    [SerializeField]
    /// <summary>
    /// Variabile che controlla la forza del knockback del nemico
    /// </summary>
    protected float enemyKnockbackForce;

    /// <summary>
    /// Funzione di Setup
    /// </summary>
    public virtual void Setup()
    {
        bulletCollider = GetComponent<Collider>();
        CalculateRaySpacing();
        UpdateRaycastOrigins();
    }

    /// <summary>
    /// Funzione che richiama l'evento di spawn del proiettile
    /// </summary>
    protected virtual void ObjectSpawnEvent()
    {
        if (OnObjectSpawn != null)
            OnObjectSpawn(this);
    }
    /// <summary>
    /// Funzione che richiama l'evento di Destroy del proiettile
    /// </summary>
    protected virtual void ObjectDestroyEvent()
    {
        if (OnObjectDestroy != null)
            OnObjectDestroy(this);
    }

    /// <summary>
    /// Funzione che gestisce il behaviour del proiettile
    /// </summary>
    protected abstract void Move();

    private void Update()
    {
        if (CurrentState == State.InUse)
        {
            Move();
        }
    }

    #region IBullet
    /// <summary>
    /// Funzione che inizializza il proiettile e lo fa sparare
    /// </summary>
    /// <param name="_speed"></param>
    /// <param name="_range"></param>
    /// <param name="_shootPosition"></param>
    /// <param name="_direction"></param>
    public virtual void Shot(int _damage, float _speed, float _range, Vector3 _shotPosition, Vector3 _direction)
    {
        damage = _damage;
        speed = _speed;
        range = _range;
        shotDirection = _direction;
        shotPosition = _shotPosition;
        targetPosition = null;
        transform.position = shotPosition;

        shotAngle = Mathf.Atan2(shotDirection.Value.y, shotDirection.Value.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, shotAngle);
        ObjectSpawnEvent();
    }

    /// <summary>
    /// Funzione che inizializza il proiettile e lo fa sparare ad un target
    /// </summary>
    /// <param name="_speed"></param>
    /// <param name="_shotPosition"></param>
    /// <param name="_target"></param>
    public virtual void Shot(int _damage, float _speed, float _range, Vector3 _shotPosition, Transform _target)
    {
        damage = _damage;
        speed = _speed;
        range = _range;
        shotPosition = _shotPosition;
        targetPosition = _target.position;
        shotDirection = null;
        transform.position = shotPosition;

        ObjectSpawnEvent();
    }

    /// <summary>
    /// Funzione che ritrona il danno che fa il proiettile
    /// </summary>
    /// <returns></returns>
    public int GetBulletDamage()
    {
        return damage;
    }
    #endregion

    #region Collision
    Collider bulletCollider;
    const float colliderOffset = 0.1f;
    [Header("Collision Settings")]
    [SerializeField]
    int rayCount;
    [SerializeField]
    float rayLenght;
    float raySpacing;
    RaycastStartPoints raycastPoint;

    /// <summary>
    /// Funzione che calcola lo spazio tra i raycast
    /// </summary>
    private void CalculateRaySpacing()
    {
        Bounds bulletBound = bulletCollider.bounds;
        bulletBound.Expand(colliderOffset * -2f);
        raySpacing = bulletBound.size.y / (rayCount - 1);
    }

    /// <summary>
    /// Funzione che calcola i 4 punti principali da cui partono i raycast
    /// AltoDestra, AltoSinistra, BassoDestra, BassoSinistra
    /// </summary>
    private void UpdateRaycastOrigins()
    {
        Bounds bounds = bulletCollider.bounds;
        bounds.Expand(colliderOffset * -2);

        raycastPoint.bottomLeft = new GameObject("BottomLeftPoint").transform;
        raycastPoint.bottomRight = new GameObject("BottomRightPoint").transform;
        raycastPoint.topLeft = new GameObject("TopLeftPoint").transform;
        raycastPoint.topRight = new GameObject("TopRightPoint").transform;

        raycastPoint.bottomLeft.SetParent(transform);
        raycastPoint.bottomRight.SetParent(transform);
        raycastPoint.topLeft.SetParent(transform);
        raycastPoint.topRight.SetParent(transform);

        raycastPoint.bottomLeft.position = new Vector3(bounds.min.x, bounds.min.y, transform.position.z);
        raycastPoint.bottomRight.position = new Vector3(bounds.max.x, bounds.min.y, transform.position.z);
        raycastPoint.topLeft.position = new Vector3(bounds.min.x, bounds.max.y, transform.position.z);
        raycastPoint.topRight.position = new Vector3(bounds.max.x, bounds.max.y, transform.position.z);
    }

    /// <summary>
    /// Funzione che controlla se avviene una collisione sugli assi verticali
    /// </summary>
    /// <param name="_movementVelocity"></param>
    protected bool Checkcollisions(Vector3 _direction)
    {
        rayLenght = Mathf.Abs(_direction.x) + colliderOffset;
        //Cicla tutti i punti da cui deve partire un raycast
        for (int i = 0; i < rayCount; i++)
        {
            //Determina il punto da cui deve partire il ray (centro del proiettile)
            //Vector3 rayOrigin = transform.position - transform.up * (raySpacing * ((rayCount - 1) / 2)) + (transform.right * offSet);
            Vector3 rayOrigin = raycastPoint.topLeft.position;
            rayOrigin += -transform.right * (raySpacing * i);

            //Crea il ray            
            Ray ray = new Ray(rayOrigin, transform.right);
            RaycastHit hit;

            //Eseguo il raycast
            if (Physics.Raycast(ray, out hit, rayLenght))
            {
                //Se colpisce qualcosa chiama la funzione e ritorna il valore di ritorno di OnBulletCollision            
                return OnBulletCollision(hit);
            }

            Debug.DrawRay(rayOrigin, transform.right * rayLenght, Color.red);
        }
        return false;
    }

    /// <summary>
    /// Funzione chiamata quando il proiettile entra in collisione con qualcosa
    /// </summary>
    /// <param name="_collisionInfo"></param>
    protected virtual bool OnBulletCollision(RaycastHit _collisionInfo)
    {
        if (
            ownerObject != null &&
            (
            _collisionInfo.transform.GetComponent<IBullet>() != null ||
            _collisionInfo.transform.gameObject == gameObject ||
            _collisionInfo.transform.gameObject == ownerObject ||
            _collisionInfo.transform.gameObject.layer == LayerMask.NameToLayer("Checkpoint") ||
             _collisionInfo.transform.gameObject.layer == LayerMask.NameToLayer("EnemyLimitLayer") ||
            ownerObject.tag == "Player" && _collisionInfo.transform.gameObject.layer == LayerMask.NameToLayer("Player") ||
            ownerObject.tag == "PlayerImmunity" && _collisionInfo.transform.gameObject.layer == LayerMask.NameToLayer("PlayerImmunity")
            )
          )
            return false;

        ObjectDestroyEvent();
        return true;
    }

    /// <summary>
    /// Struttura che contiene le coordinate dei 4 punti principali da cui partono i ray
    /// che controllano le collisioni
    /// </summary>
    private struct RaycastStartPoints
    {
        public Transform topLeft;
        public Transform topRight;
        public Transform bottomLeft;
        public Transform bottomRight;
    }
    #endregion

    private void OnDisable()
    {
        OnObjectSpawn = null;
        OnObjectDestroy = null;
    }
}
