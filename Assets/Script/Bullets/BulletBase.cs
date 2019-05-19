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
    protected float damage;
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
    /// <summary>
    /// Bullet explosion behaviour (può essere null)
    /// </summary>
    protected BulletExplosionBehaviour bulletExplosion;
    /// <summary>
    /// Riferimento al collider del bullet
    /// </summary>
    protected SphereCollider bulletCollider;

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
        bulletExplosion = GetComponent<BulletExplosionBehaviour>();
        bulletCollider = GetComponent<SphereCollider>();
        bulletCollider.enabled = false;
    }

    /// <summary>
    /// Funzione che richiama l'evento di spawn del proiettile
    /// </summary>
    protected virtual void ObjectSpawnEvent()
    {
        bulletCollider.enabled = true;

        if (OnObjectSpawn != null)
            OnObjectSpawn(this);
    }
    /// <summary>
    /// Funzione che richiama l'evento di Destroy del proiettile
    /// </summary>
    protected virtual void ObjectDestroyEvent()
    {
        bulletCollider.enabled = false;

        if (bulletExplosion != null)
            bulletExplosion.Explode(ownerObject, damage);

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
    public virtual void Shot(float _damage, float _speed, float _range, Vector3 _shotPosition, Vector3 _direction)
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
    public virtual void Shot(float _damage, float _speed, float _range, Vector3 _shotPosition, Transform _target)
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
    public float GetBulletDamage()
    {
        return damage;
    }
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        OnBulletCollision(other);
    }

    /// <summary>
    /// Funzione chiamata quando il proiettile entra in collisione con qualcosa
    /// </summary>
    /// <param name="_collisionInfo"></param>
    protected virtual bool OnBulletCollision(Collider _collider)
    {
        if (
            ownerObject != null &&
            (
            _collider.gameObject.GetComponent<IBullet>() != null ||
            _collider.gameObject == gameObject ||
            _collider.gameObject == ownerObject ||
            _collider.gameObject.layer == LayerMask.NameToLayer("Checkpoint") ||
            _collider.gameObject.layer == LayerMask.NameToLayer("EnemyLimitLayer") ||
            ownerObject.tag == "Player" && _collider.gameObject.layer == LayerMask.NameToLayer("Player") ||
            ownerObject.tag == "PlayerImmunity" && _collider.gameObject.layer == LayerMask.NameToLayer("PlayerImmunity")
            )
          )
            return false;

        ObjectDestroyEvent();
        return true;
    }

    private void OnDisable()
    {
        OnObjectSpawn = null;
        OnObjectDestroy = null;
    }
}
